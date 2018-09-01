using System;
using System.Windows.Forms;

namespace EveClipboardWatcher
{
    public partial class OptionsForm : Form
    {
        private MainForm m_mf;
        public OptionsForm(MainForm form)
        {
            InitializeComponent();

            m_mf = form;
            cbTopMost.Checked = m_mf.TopMost;
            trackBarOpaque.Value = (int)(m_mf.Opacity * 100);

            SecOptions o = SecOptions.getInstance();
            o.readKey();
            tbClientId.Text = o.m_clientId;
            tbSecretKey.Text = o.m_secretKey;
        }

        private void cbTopMost_CheckStateChanged(object sender, EventArgs e)
        {
            m_mf.TopMost = cbTopMost.Checked;
        }

        private void trackBarOpaque_ValueChanged(object sender, EventArgs e)
        {
            m_mf.Opacity = (double)trackBarOpaque.Value / 100;
        }

        private void OptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SecOptions o = SecOptions.getInstance();
            o.m_clientId = tbClientId.Text;
            o.m_secretKey = tbSecretKey.Text;
            o.writeKey();
        }
    }
}
