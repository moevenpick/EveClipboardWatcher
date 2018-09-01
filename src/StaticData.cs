using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace EveClipboardWatcher
{
    class StaticData
    {
        public dynamic m_typeIds { get; set; }
        public dynamic m_groupIds { get; set; }
        public bool m_loaded { get; set; }

        private static StaticData m_sd;

        private Thread m_worker;

        public static StaticData getInstance()
        {
            if (m_sd == null)
            {
                m_sd = new StaticData();
                m_sd.m_worker = new Thread(new ParameterizedThreadStart(m_sd.worker_LoadData));
                m_sd.m_worker.Start(m_sd);
            }
            return m_sd;
        }

        private StaticData()
        {
            m_loaded = false;
        }

        private void worker_LoadData(object obj)
        {
            string text = System.IO.File.ReadAllText(@"data/typeIDs.yaml");
            StringReader sr = new StringReader(text);
            Deserializer deserializer = new Deserializer();
            m_typeIds = deserializer.Deserialize(sr);

            text = System.IO.File.ReadAllText(@"data/groupIDs.yaml");
            sr = new StringReader(text);
            deserializer = new Deserializer();
            m_groupIds = deserializer.Deserialize(sr);

            m_loaded = true;
        }
    }
}
