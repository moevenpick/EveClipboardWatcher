using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.SSO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EveClipboardWatcher
{
    class EsiData
    {
        public EsiClient m_client;
        public AuthorizedCharacterData m_authCharData;
        private MainForm m_mainForm;
        private List<Thread> m_personThreads = new List<Thread>();

        public EsiData(MainForm mainform)
        {
            SecOptions o = SecOptions.getInstance();
            o.readKey();

            IOptions<EsiConfig> config = Options.Create(new EsiConfig()
            {
                EsiUrl = "https://esi.tech.ccp.is/",
                DataSource = DataSource.Tranquility,
                ClientId = o.m_clientId, 
                SecretKey = o.m_secretKey, 
                CallbackUrl = "eveauth-eveclip://callback/",
                UserAgent = "Name: Che Silas"
            });

            m_client = new EsiClient(config);
            m_authCharData = null;
            m_mainForm = mainform;
            InitAuth();
        }

        public void InitAuth()
        {
            var url = m_client.SSO.CreateAuthenticationUrl(new List<string> {
                "publicData",
                //"esi-calendar.respond_calendar_events.v1",
                //"esi-calendar.read_calendar_events.v1",
                "esi-location.read_location.v1",
                "esi-location.read_ship_type.v1",
                //"esi-mail.organize_mail.v1",
                //"esi-mail.read_mail.v1",
                //"esi-mail.send_mail.v1",
                //"esi-skills.read_skills.v1",
                //"esi-skills.read_skillqueue.v1",
                //"esi-wallet.read_character_wallet.v1",
                //"esi-wallet.read_corporation_wallet.v1",
                "esi-search.search_structures.v1",
                //"esi-clones.read_clones.v1",
                //"esi-characters.read_contacts.v1",
                //"esi-universe.read_structures.v1",
                //"esi-bookmarks.read_character_bookmarks.v1",
                "esi-killmails.read_killmails.v1",
                //"esi-corporations.read_corporation_membership.v1",
                //"esi-assets.read_assets.v1",
                //"esi-planets.manage_planets.v1",
                //"esi-fleets.read_fleet.v1",
                //"esi-fleets.write_fleet.v1",
                //"esi-ui.open_window.v1",
                //"esi-ui.write_waypoint.v1",
                //"esi-characters.write_contacts.v1",
                //"esi-fittings.read_fittings.v1",
                //"esi-fittings.write_fittings.v1",
                //"esi-markets.structure_markets.v1",
                //"esi-corporations.read_structures.v1",
                //"esi-corporations.write_structures.v1",
                //"esi-characters.read_loyalty.v1",
                //"esi-characters.read_opportunities.v1",
                //"esi-characters.read_chat_channels.v1",
                //"esi-characters.read_medals.v1",
                //"esi-characters.read_standings.v1",
                //"esi-characters.read_agents_research.v1",
                //"esi-industry.read_character_jobs.v1",
                //"esi-markets.read_character_orders.v1",
                //"esi-characters.read_blueprints.v1",
                //"esi-characters.read_corporation_roles.v1",
                //"esi-location.read_online.v1",
                //"esi-contracts.read_character_contracts.v1",
                //"esi-clones.read_implants.v1",
                //"esi-characters.read_fatigue.v1",
                //"esi-killmails.read_corporation_killmails.v1",
                //"esi-corporations.track_members.v1",
                //"esi-wallet.read_corporation_wallets.v1",
                //"esi-characters.read_notifications.v1",
                //"esi-corporations.read_divisions.v1",
                //"esi-corporations.read_contacts.v1",
                //"esi-assets.read_corporation_assets.v1",
                //"esi-corporations.read_titles.v1",
                //"esi-corporations.read_blueprints.v1",
                //"esi-bookmarks.read_corporation_bookmarks.v1",
                //"esi-contracts.read_corporation_contracts.v1",
                //"esi-corporations.read_standings.v1",
                //"esi-corporations.read_starbases.v1",
                //"esi-industry.read_corporation_jobs.v1",
                //"esi-markets.read_corporation_orders.v1",
                //"esi-corporations.read_container_logs.v1",
                //"esi-industry.read_character_mining.v1",
                //"esi-industry.read_corporation_mining.v1",
                //"esi-planets.read_customs_offices.v1",
                //"esi-corporations.read_facilities.v1",
                //"esi-corporations.read_medals.v1",
                //"esi-characters.read_titles.v1",
                //"esi-alliances.read_contacts.v1",
                //"esi-characters.read_fw_stats.v1",
                //"esi-corporations.read_fw_stats.v1",
                //"esi-corporations.read_outposts.v1",
                "esi-characterstats.read.v1"
            });

            System.Diagnostics.Process.Start(url);
        }

        public async void finishAuth(string uri)
        { 
            // eveauth-eveclip://callback/?code=DX58B9i1IkvZj23aoAZPXQmNR1NZi7hixM0Vm2lWXV71zl1-Dlpkd1jUgS4gl0tq0
            int pos = uri.IndexOf("?code=");
            string code = uri.Substring(pos + 6);
            try
            {
                SsoToken token = await m_client.SSO.GetToken(GrantType.AuthorizationCode, code);
                m_authCharData = await m_client.SSO.Verify(token.Value);
                m_client.SetCharacterData(m_authCharData);
            }
            catch(Exception)
            {
                 MessageBox.Show("Login failed. Please enter ClientID and SecretKey in Options. See https://developers.eveonline.com/ for details.", "Error logging in");
            }
        }

        internal void setCurrentUserAndShip()
        {
            Task<EsiResponse<ESI.NET.Models.Location.Ship>> shipTask = null;
            shipTask = Task.Run<EsiResponse<ESI.NET.Models.Location.Ship>>(async () => await m_client.Location.Ship());
            shipTask.Wait();

            try
            {
                StaticData sd = StaticData.getInstance();
                m_mainForm.m_ownShipTypeId = String.Format("{0}", shipTask.Result.Data.ShipTypeId);
                string groupId = sd.m_typeIds[m_mainForm.m_ownShipTypeId]["groupID"];
                m_mainForm.m_ownShipTypeName = sd.m_typeIds[m_mainForm.m_ownShipTypeId]["name"]["en"];
                m_mainForm.m_ownShipCategory = sd.m_groupIds[groupId]["name"]["en"];
                //m_mainForm.statusStrip1.Invoke((MethodInvoker)delegate { m_mainForm.UpdateStatus(false, false, false, false); });
            }
            catch(Exception)
            {
                //something weird with static data
            }
        }

        internal void fillChar(PilotRow pilotrow)
        {
            Thread t = new Thread(new ParameterizedThreadStart(fillCharThread));
            t.Start(pilotrow);
            m_personThreads.Add(t);
        }

        void fillCharThread(object pilotrowobj)
        {
            PilotRow pilotrow = pilotrowobj as PilotRow;
            try
            {
                ESI.NET.Models.SearchResults character = searchItem(pilotrow.m_name, SearchCategory.Character);
                long id = character.Characters[0];
                pilotrow.m_id = String.Format("{0}", id);

                downloadImage(1, pilotrow.m_id);

                Task<EsiResponse<ESI.NET.Models.Character.Information>> charTask = Task.Run<EsiResponse<ESI.NET.Models.Character.Information>>(async () => await m_client.Character.Information((int)id));
                charTask.Wait();

                pilotrow.m_birthday = charTask.Result.Data.Birthday.ToShortDateString();
                pilotrow.m_secstatus = String.Format("{0}", charTask.Result.Data.SecurityStatus);

            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                m_mainForm.statusStrip1.Invoke((MethodInvoker)delegate { m_mainForm.UpdateStatus(false, false, false, true); });
            }

            // be gentle
            Thread.Sleep(500);

            // getting killmails from other users
            // https://zkillboard.com/api/kills/characterID/1792033579/no-items/
            // https://zkillboard.com/api/losses/characterID/1792033579/no-items/no-attackers/
            string resp = new WebClient().DownloadString("https://zkillboard.com/api/kills/characterID/" + pilotrow.m_id + "/no-items/");

            if (resp.Length <= 0 || !resp.StartsWith("["))
            {
                MessageBox.Show(resp, "Error from zkillboard.com");
                return;
            }

            dynamic page = JsonConvert.DeserializeObject(resp);
            pilotrow.m_kills = page.Count;

            // check current ship and group members
            if(pilotrow.m_kills > 0)
            {
                //check date "11/29/2016 00:33:02" "2018-08-26T01:20:42Z"
                string lastkill = page[0].killmail_time;
                DateTime now = DateTime.Now;
                now = now.AddHours(-5);
                DateTime myDate = DateTime.ParseExact(lastkill, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                if(myDate > now)
                {
                    pilotrow.m_fightCompanions = page[0].attackers.Count - 1;
                    for (int i=0; i<page[0].attackers.Count; i++)
                    {
                        if(page[0].attackers[i].character_id == pilotrow.m_id)
                        {
                            pilotrow.m_shipId = page[0].attackers[i].ship_type_id;
                        }
                    }
                }
            }

            resp = new WebClient().DownloadString("https://zkillboard.com/api/losses/characterID/" + pilotrow.m_id + "/no-items/no-attackers/");

            if (resp.Length <= 0 || !resp.StartsWith("["))
            {
                MessageBox.Show(resp, "Error from zkillboard.com");
                return;
            }

            page = JsonConvert.DeserializeObject(resp);
            pilotrow.m_deaths = page.Count;
            m_mainForm.statusStrip1.Invoke((MethodInvoker)delegate { m_mainForm.UpdateStatus(false, false, true, true); });
        }

        internal void killAllPersonThreads()
        {
            foreach(Thread t in m_personThreads)
            {
                t.Abort();
            }
        }

        internal string getLoggedUserName()
        {
            return m_authCharData == null ? "unknown" : m_authCharData.CharacterName;
        }

        public ESI.NET.Models.SearchResults searchItem(string search, SearchCategory category)
        {
            Task<EsiResponse<ESI.NET.Models.SearchResults>> searchTask = null;
            searchTask = Task.Run<EsiResponse<ESI.NET.Models.SearchResults>>(async () => await m_client.Search.Query(EsiRequest.RequestSecurity.Authenticated, search, category, true));
            searchTask.Wait();
            return searchTask == null ? null : searchTask.Result.Data;
        }

        // nType = 0 -> ship 
        // nType = 1 -> character
        public void downloadImage(int nType, string itemId)
        {
            if (nType == 0)
            {
                //https://image.eveonline.com/Render/585_64.png
                if (!File.Exists(MainForm.m_appDataFolder + "\\" + itemId + ".png"))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://image.eveonline.com/Render/" + itemId + "_64.png", MainForm.m_appDataFolder + "\\" + itemId + ".png");
                    }
                }
            }
            else
            {
                //https://image.eveonline.com/Character/1792033579_64.jpg
                //https://image.eveonline.com/Character/2112371518_64.png
                //https://image.eveonline.com/Character/1_64.jpg
                if (!File.Exists(MainForm.m_appDataFolder + "\\char_" + itemId + ".jpg"))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://image.eveonline.com/Character/" + itemId + "_64.jpg", MainForm.m_appDataFolder + "\\char_" + itemId + ".jpg");
                    }
                }
            }
        }

    }
}
