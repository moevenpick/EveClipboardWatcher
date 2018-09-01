using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32;
using Newtonsoft.Json;
using static EveClipboardWatcher.MessageHelper;

namespace EveClipboardWatcher
{
    public partial class MainForm : Form
    {
        public static string UriScheme = "eveauth-eveclip";
        public static string FriendlyName = "Sample Protocol";
        public static String m_appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\eveclipboardwatcher";
        public static int ICONHEIGHT = 64;

        public static Color COLOR_A = Color.Green;
        public static Color COLOR_B = Color.Yellow;
        public static Color COLOR_C = Color.Orange;
        public static Color COLOR_D = Color.Red;
        private EsiData m_esi;
        public string m_ownShipTypeName;
        public string m_ownShipTypeId;
        public string m_ownShipCategory;
        static bool m_bCheckLoaded = false;
        private Thread m_loadDatabaseThread;
        private int m_npage = 0;
        private bool m_backworker_running;


        private List<ShipRow> m_ships = new List<ShipRow>();
        private List<PilotRow> m_local = new List<PilotRow>();
        private Dictionary<string, WinLoss> m_database = new Dictionary<string, WinLoss>();
        private static Mutex m_databaseMutex = new Mutex();
        private Semaphore m_semaphore = new Semaphore(0, 1);
        private string m_oldClipboardText;

        // Clipboard viewer
        [DllImport("User32.dll")]
        protected static extern int
          SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool
               ChangeClipboardChain(IntPtr hWndRemove,
                                    IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg,
                                             IntPtr wParam,
                                             IntPtr lParam);
        private IntPtr nextClipboardViewer;

        public MainForm()
        {
            InitializeComponent();
            RegisterUriScheme();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)
                         this.Handle);
            System.IO.Directory.CreateDirectory(m_appDataFolder);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;
            const int WM_CLOSE = 0x0010;


            switch (m.Msg)
            {
                case MessageHelper.WM_COPYDATA:
                    if (m_esi != null)
                    {
                        MessageHelper.COPYDATASTRUCT mystr = new MessageHelper.COPYDATASTRUCT();
                        System.Type mytype = mystr.GetType();
                        mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                        m_esi.finishAuth(mystr.lpData);

                        if (WindowState == FormWindowState.Minimized)
                        {
                            WindowState = FormWindowState.Normal;
                        }
                        // get our current "TopMost" value (ours will always be false though)
                        bool top = TopMost;
                        // make our form jump to the top of everything
                        TopMost = true;
                        // set it back to whatever it was
                        TopMost = top;
                    }
                    break;
                case WM_DRAWCLIPBOARD:
                    CheckClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam,
                                m.LParam);
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam,
                                    m.LParam);
                    break;
                case WM_CLOSE:
                    Process.GetCurrentProcess().Kill();
                    break;
            }

            // check if static data is now ready
            if (!m_bCheckLoaded && m_esi != null && m_esi.m_authCharData != null && StaticData.getInstance().m_loaded)
            {
                m_bCheckLoaded = true;
                m_esi.setCurrentUserAndShip();
                m_backworker_running = true;
                m_loadDatabaseThread = new Thread(new ParameterizedThreadStart(worker_LoadShipDataThread));
                m_loadDatabaseThread.Start(this);
            }
            base.WndProc(ref m);
        }

        private static bool bCheckClipboardDataRunning = false;

        private void CheckClipboardData()
        {
            if(bCheckClipboardDataRunning || m_esi == null || !StaticData.getInstance().m_loaded)
            {
                return;
            }

            bCheckClipboardDataRunning = true;

            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                Debug.WriteLine("CheckClipboardData()");
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);

                if(m_oldClipboardText != clipboardText)
                {
                    m_oldClipboardText = clipboardText;
                    // check if d-scan
                    bool bDScan = false;
                    MatchCollection matches = Regex.Matches(clipboardText, @"^(\d+)\t([^\t]+).*$", RegexOptions.Multiline);
                    if(matches.Count > 0)
                    {
                        m_ships.Clear();
                    }
                    foreach (Match match in matches)
                    {
                        bDScan = true;

                        ShipRow shiprow = new ShipRow();
                        shiprow.m_name = match.Groups[2].Value.Trim();
                        shiprow.m_typeid = match.Groups[1].Value.Trim();

                        try
                        {
                            StaticData sd = StaticData.getInstance();
                            string groupId = sd.m_typeIds[shiprow.m_typeid]["groupID"];
                            shiprow.m_typename = sd.m_typeIds[shiprow.m_typeid]["name"]["en"];
                            shiprow.m_faction = sd.m_typeIds[shiprow.m_typeid]["sofFactionName"];
                            shiprow.m_category = sd.m_groupIds[groupId]["name"]["en"];
                            m_ships.Add(shiprow);

                            m_esi.downloadImage(0, shiprow.m_typeid);
                        } 
                        catch(Exception)
                        {
                            // try next row
                        }
                    }
                    if (bDScan)
                    {
                        //dataGridViewMain.Invoke((MethodInvoker)delegate { UpdateStatus(true, false, true, false); });
                        UpdateStatus(true, false, true, false);
                    }

                    // check local persons
                    bool bLocalScan = false;
                    if (!bDScan)
                    {
                        string ownname = m_esi.getLoggedUserName();
                        clipboardText += "\r\n"; // just to be sure
                        matches = Regex.Matches(clipboardText, @"^(.*)$", RegexOptions.Multiline);
                        foreach (Match match in matches)
                        {
                            if (match.Groups[1].Value.Trim() == ownname)
                            {
                                bLocalScan = true;
                                break;
                            }
                        }
                        if (bLocalScan)
                        {
                            m_local.Clear();
                            m_esi.killAllPersonThreads();
                            foreach (Match match in matches)
                            {
                                string name = match.Groups[1].Value.Trim();
                                if(name.Length > 0 && name != ownname)
                                {
                                    PilotRow pilotrow = new PilotRow();
                                    pilotrow.m_name = name;
                                    m_esi.fillChar(pilotrow);
                                    m_local.Add(pilotrow);
                                }
                            }
                            //dataGridViewMain.Invoke((MethodInvoker)delegate { UpdateStatus(false, true, false, true); });
                            UpdateStatus(false, true, false, true);
                        }
                    }
                }
            }
            bCheckClipboardDataRunning = false;

        }

        public static void RegisterUriScheme()
        {
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
            {
                string applicationLocation = System.Reflection.Assembly.GetExecutingAssembly().Location; // typeof(App).Assembly.Location;

                key.SetValue("", "URL:" + FriendlyName);
                key.SetValue("URL Protocol", "");

                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("", "\"" + applicationLocation + ",1\"");
                }

                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                }
            }
        }

        private void worker_LoadShipDataThread(object sender)
        {
            m_npage = 1;
            m_databaseMutex.WaitOne();
            try
            {
                m_database = new Dictionary<string, WinLoss>();
                WinLoss wl = new WinLoss();
                wl.m_wins = 1;
                wl.m_deaths = 1;
                m_database.Add(m_ownShipTypeId, wl);
            }
            finally
            {
                m_databaseMutex.ReleaseMutex();
            }

            while (m_backworker_running)
            {
                try
                {
                    Debug.WriteLine("worker_LoadShipData() get HTML");
                    DateTime dt = DateTime.Now;
                    dt = dt.AddYears(-1);
                    string lastyear = dt.ToString("yyyyMMdd0000");
                    string xpage = String.Format("{0}", m_npage);
                    //https://zkillboard.com/api/solo/shipTypeID/585/startTime/201709010000/page/19/
                    string resp = new WebClient().DownloadString("https://zkillboard.com/api/solo/shipTypeID/" + m_ownShipTypeId + "/startTime/" + lastyear + "/page/" + xpage + "/");

                    if (resp.Length <= 0 || !resp.StartsWith("["))
                    {
                        break;
                    }

                    dynamic page = JsonConvert.DeserializeObject(resp);

                    for (int i = 0; m_backworker_running && i < page.Count; i++)
                    {
                        string attacker = page[i].attackers[0].ship_type_id;
                        string attackerw = page[i].attackers[0].weapon_type_id;
                        string victim = page[i].victim.ship_type_id;
                        //Debug.WriteLine("Attacker: " + attacker + " weapon: " + attackerw + " victim: " + victim);

                        if(attacker == null || victim == null)
                        {
                            continue;
                        }
                        m_databaseMutex.WaitOne();
                        try
                        {
                            if (attacker == m_ownShipTypeId && victim == m_ownShipTypeId)
                            {
                                continue;
                            }
                            else if (attacker == m_ownShipTypeId)
                            {
                                WinLoss otherShip = null;
                                if (m_database.ContainsKey(victim))
                                {
                                    otherShip = m_database[victim];
                                }
                                else
                                {
                                    otherShip = new WinLoss();
                                    m_database.Add(victim, otherShip);
                                }
                                otherShip.m_deaths++;
                            }
                            else if (victim == m_ownShipTypeId)
                            {
                                WinLoss otherShip = null;
                                if (m_database.ContainsKey(attacker))
                                {
                                    otherShip = m_database[attacker];
                                }
                                else
                                {
                                    otherShip = new WinLoss();
                                    m_database.Add(attacker, otherShip);
                                }
                                otherShip.m_wins++;
                            }
                        }
                        finally
                        {
                            m_databaseMutex.ReleaseMutex();
                        }
                    }


                    m_npage++;
                    Thread.Sleep(2000);
                    // gives a race condition when changing the vessel
                    //dataGridViewMain.Invoke((MethodInvoker)delegate { UpdateStatus(false, false, false, false); });
                }
                catch (Exception)
                {

                }
            }
            m_backworker_running = true;
            m_semaphore.Release();
        }

        private void btOptions_Click(object sender, EventArgs e)
        {
            // show options dialog
            OptionsForm of = new OptionsForm(this);
            of.ShowDialog();
        }

        private void tsbtLogin_Click(object sender, EventArgs e)
        {
            m_esi = null;
            m_bCheckLoaded = false;
            m_esi = new EsiData(this);
            StaticData.getInstance();
            UpdateStatus(false, false, false, false);
        }

        public void UpdateStatus(bool bNewDScanGrid, bool bNewLocalGrid, bool bUpdateDScan, bool bUpdateLocal)
        {
            if(m_esi == null)
            {
                toolStripStatusLabel.Text = String.Format("Not logged in");
                return;
            }
            else if (m_esi.m_client == null)
            {
                toolStripStatusLabel.Text = String.Format("Couldn't login, check ClientID and SecretKey in options");
                return;
            }
            else if (!StaticData.getInstance().m_loaded)
            {
                toolStripStatusLabel.Text = String.Format("Loading static data (takes up to 2') ...");
                return;
            }
            else if (m_ownShipTypeId is null || m_ownShipTypeId.Length <= 0)
            {
                toolStripStatusLabel.Text = String.Format("Couldn't find your current ship in static data");
                return;
            }
            else
            {
                toolStripStatusLabel.Text = String.Format("Logged in with user {0} current ship {1} {2} | update database for your ship from zkillboard.com page {3}", m_esi.getLoggedUserName(), m_ownShipTypeName, m_ownShipCategory, m_npage);
            }
            Debug.WriteLine("UpdateStatus " + (bNewDScanGrid ? "J":"N") + (bNewLocalGrid ? "J" : "N") + (bUpdateDScan ? "J" : "N") + (bUpdateLocal ? "J" : "N"));
            if (bUpdateDScan)
            {
                if (bNewDScanGrid)
                {
                    dataGridViewMain.Rows.Clear();
                }
                for (int i = 0; i < m_ships.Count; i++)
                {
                    ShipRow shiprow = m_ships[i];
                    DataGridViewRow row = shiprow.m_row;
                    if (bNewDScanGrid)
                    {
                        dataGridViewMain.Rows.Add();
                        row = dataGridViewMain.Rows[i];
                        row.Height = ICONHEIGHT;
                    }
                    try
                    {
                        Image bmp = Image.FromFile(MainForm.m_appDataFolder + "\\" + shiprow.m_typeid + ".png");
                        //bmp = ResizeImage(bmp, ICONHEIGHT, ICONHEIGHT);
                        row.Cells[0].Value = bmp;
                    }
                    catch (Exception)
                    {
                        File.Delete(MainForm.m_appDataFolder + "\\" + shiprow.m_typeid + ".png");
                        Image bmp = new Bitmap(1, 1);
                        row.Cells[0].Value = bmp;
                    }

                    row.Cells[1].Value = shiprow.m_name + "\r\n\r\n" + shiprow.m_typename;
                    row.Cells[2].Value = shiprow.m_category + "\r\n\r\n" + shiprow.m_faction;
                    m_databaseMutex.WaitOne();
                    try
                    {
                        WinLoss wl = m_database[shiprow.m_typeid];
                        row.Cells[3].Value = shiprow.getShipStatus(wl);
                    }
                    catch (Exception)
                    {
                        row.Cells[3].Value = "n/a";
                    }
                    finally
                    {
                        m_databaseMutex.ReleaseMutex();
                    }

                    row.Cells[4].Value = "";
                    for (int ii = 0; ii < m_local.Count; ii++)
                    {
                        if (m_local[ii].m_shipId == shiprow.m_typeid)
                        {
                            row.Cells[4].Value += String.Format("{0} ({1}) #group({2})\r\n", m_local[ii].m_name, m_local[ii].getExpirence(), m_local[ii].m_fightCompanions); 
                        }
                    }

                    row.Cells[5].Value = shiprow.getRatio();
                    switch(shiprow.m_ratio)
                    {
                        case ShipRow.SHIP_RATIO.SHOOT_HIM:
                            row.Cells[5].Style.BackColor = COLOR_A;
                            break;
                        case ShipRow.SHIP_RATIO.TRY_IT:
                            row.Cells[5].Style.BackColor = COLOR_B;
                            break;
                        case ShipRow.SHIP_RATIO.DONT_TRY_IT:
                            row.Cells[5].Style.BackColor = COLOR_C;
                            break;
                        case ShipRow.SHIP_RATIO.YOU_DIE:
                            row.Cells[5].Style.BackColor = COLOR_D;
                            break;
                    }

                    row.Tag = shiprow;
                    shiprow.m_row = row;
                }
            }

            if (bUpdateLocal)
            {
                if (bNewLocalGrid)
                {
                    dataGridViewLocal.Rows.Clear();
                }
                for (int i = 0; i < m_local.Count; i++)
                {
                    PilotRow pilot = m_local[i];
                    DataGridViewRow row = pilot.m_row;

                    if (bNewLocalGrid)
                    {
                        dataGridViewLocal.Rows.Add();
                        row = dataGridViewLocal.Rows[i];
                        row.Height = ICONHEIGHT;
                    }
                    try
                    {
                        Image bmp = null;
                        string filename = MainForm.m_appDataFolder + "\\char_" + pilot.m_id + ".jpg";
                        if (File.Exists(filename))
                        {
                            bmp = Image.FromFile(MainForm.m_appDataFolder + "\\char_" + pilot.m_id + ".jpg");
                            //bmp = ResizeImage(bmp, ICONHEIGHT, ICONHEIGHT);
                        }
                        else
                        {
                            bmp = new Bitmap(1, 1);
                        }
                        row.Cells[0].Value = bmp;

                    }
                    catch (Exception)
                    {
                        Image bmp = new Bitmap(1, 1);
                        row.Cells[0].Value = bmp;
                    }

                    row.Cells[1].Value = pilot.m_name + "\r\n" + pilot.m_birthday + "\r\n" + pilot.m_secstatus;

                    if (pilot.m_deaths > 0)
                    {
                        double ratio = (double)pilot.m_kills / (double)pilot.m_deaths;
                        row.Cells[2].Value = String.Format("{0}/{1}\r\n\r\n{2:0.00}", pilot.m_kills, pilot.m_deaths, ratio);
                    }
                    else
                    {
                        row.Cells[2].Value = String.Format("{0}/0\r\n\r\n{1:0.00}", pilot.m_kills, pilot.m_kills);
                    }
                    row.Cells[3].Value = pilot.getExpirence();
                    switch (pilot.m_statusCache)
                    {
                        case PilotRow.PILOT_EXP.NOVICE:
                            row.Cells[3].Style.BackColor = COLOR_A;
                            break;
                        case PilotRow.PILOT_EXP.ADVANCED:
                            row.Cells[3].Style.BackColor = COLOR_B;
                            break;
                        case PilotRow.PILOT_EXP.EXPERT:
                            row.Cells[3].Style.BackColor = COLOR_C;
                            break;
                        case PilotRow.PILOT_EXP.VETERAN:
                            row.Cells[3].Style.BackColor = COLOR_D;
                            break;
                    }

                    row.Tag = pilot;
                    pilot.m_row = row;
                }
            }

        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AboutForm dlg = new AboutForm();
            dlg.ShowDialog();
        }

        private void dataGridViewLocal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                PilotRow pilotrow = dataGridViewLocal.Rows[e.RowIndex].Tag as PilotRow;
                //https://zkillboard.com/character/91307906/
                string url = "https://zkillboard.com/character/" + pilotrow.m_id + "/";
                System.Diagnostics.Process.Start(url);
            }
        }

        private void btChangeShipType_Click_1(object sender, EventArgs e)
        {
            if (m_esi != null && m_esi.m_authCharData != null && StaticData.getInstance().m_loaded)
            {
                m_esi.killAllPersonThreads();
                m_backworker_running = false;
                m_semaphore.WaitOne();
                m_npage = 0;
                m_bCheckLoaded = true;
                m_esi.setCurrentUserAndShip();
                m_loadDatabaseThread = new Thread(new ParameterizedThreadStart(worker_LoadShipDataThread));
                m_loadDatabaseThread.Start(this);
                UpdateStatus(true, true, true, true);
            }
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            UpdateStatus(true, true, true, true);
        }
    }

    class WinLoss
    {
        public int m_wins;
        public int m_deaths;
    }

    class ShipRow 
    {
        public enum SHIP_RATIO
        {
            SHOOT_HIM,
            TRY_IT,
            DONT_TRY_IT,
            YOU_DIE
        };

        public string m_name;
        public string m_typeid;
        public string m_typename;
        public string m_category;
        public string m_faction;
        public SHIP_RATIO m_ratio;
        public DataGridViewRow m_row;

        public List<PilotRow> m_pilots = new List<PilotRow>();

        public string getShipStatus(WinLoss wl)
        {
            ShipRow.SHIP_RATIO status = ShipRow.SHIP_RATIO.SHOOT_HIM;
            string ret;
            
            if (wl.m_deaths > 0)
            {
                double ratio = (double)wl.m_wins / (double)wl.m_deaths;
                ret = String.Format("{0}/{1}\r\n\r\n{2:0.00}", wl.m_wins, wl.m_deaths, ratio);
                if (ratio > 1)
                {
                    if (ratio < 1.3)
                    {
                        status = ShipRow.SHIP_RATIO.DONT_TRY_IT;
                    }
                    else
                    {
                        status = ShipRow.SHIP_RATIO.YOU_DIE;
                    }
                }
                else
                {
                    if (ratio > 0.7)
                    {
                        status = ShipRow.SHIP_RATIO.TRY_IT;
                    }
                }
            }
            else
            {
                ret = String.Format("{0}/0\r\n\r\n{0:0.00}", wl.m_wins);
                status = SHIP_RATIO.YOU_DIE;
            }
            m_ratio = status;
            return ret;
         }

        public string getRatio()
        {
            switch (m_ratio) {
                case SHIP_RATIO.SHOOT_HIM:
                    return "A: shoot him";
                case SHIP_RATIO.TRY_IT:
                    return "B: try it";
                case SHIP_RATIO.DONT_TRY_IT:
                    return "C: don't try it";
                default:
                    return "D: you die";
            } 
        }
    }

    class PilotRow
    {
        public string m_name;
        public string m_id;
        public int m_kills;
        public int m_deaths;
        public string m_birthday;
        public string m_secstatus;
        public DataGridViewRow m_row;
        public string m_shipId;
        public int m_fightCompanions;
        public PILOT_EXP m_statusCache;

        public enum PILOT_EXP {
            NOVICE,
            ADVANCED,
            EXPERT,
            VETERAN
        };

        public string getExpirence()
        {
            m_statusCache = PILOT_EXP.NOVICE;
            double ratio = (double)m_kills/ (m_deaths==0?1:(double)m_deaths);
            int sum = m_kills + m_deaths;

            if (ratio < 0.5)
            {
                m_statusCache = PILOT_EXP.NOVICE;
            }
            else if (ratio < 1)
            {
                m_statusCache = PILOT_EXP.ADVANCED;
            }
            else if (ratio < 2)
            {
                m_statusCache = PILOT_EXP.EXPERT;
            }
            else
            {
                m_statusCache = PILOT_EXP.VETERAN;
            }

            if(sum > 100)
            {
                switch (m_statusCache)
                {
                    case PILOT_EXP.NOVICE:
                        m_statusCache = PILOT_EXP.ADVANCED;
                        break;
                    case PILOT_EXP.ADVANCED:
                        m_statusCache = PILOT_EXP.EXPERT;
                        break;
                    case PILOT_EXP.EXPERT:
                        m_statusCache = PILOT_EXP.VETERAN;
                        break;
                }
            }

            switch (m_statusCache)
            {
                case PILOT_EXP.NOVICE:
                    return "A: Novice";
                case PILOT_EXP.ADVANCED:
                    return "B: Advanced";
                case PILOT_EXP.EXPERT:
                    return "c: Expert";
                case PILOT_EXP.VETERAN:
                default:
                    return "D: Veteran";
            }
        }
    }

    class SecOptions
    {
        private static SecOptions m_options;

        public string m_secretKey;
        public string m_clientId;

        public static SecOptions getInstance()
        {
            if(m_options == null)
            {
                m_options = new SecOptions();
            }
            return m_options;
        }

        public SecOptions()
        {

        }

        public void writeKey()
        {
            string s = m_secretKey + "\n" + m_clientId;
            string b64 = Base64Encode(Reverse(s));
            System.IO.File.WriteAllText(MainForm.m_appDataFolder + "/system.dat", b64);
        }

        public void readKey()
        {
            string filename = MainForm.m_appDataFolder + "/system.dat";
            if (File.Exists(filename))
            { 
                string b64 = System.IO.File.ReadAllText(filename);
                string s = Reverse(Base64Decode(b64));
                string[] both = s.Split('\n');
                m_secretKey = both[0];
                m_clientId = both[1];
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
