using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.BotToVisio
{
    public partial class BotToVisioCtl : PluginControlBase
    {
        #region Ctrl Load

        private Settings mySettings;

        internal List<Bot> bots { get; private set; }

        public BotToVisioCtl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            ExecuteMethod(LoadBots);
        }
        #endregion

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }


        #region Events
        private void cboBot_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(LoadTopics);
        }


        #endregion

        #region Bot retrieval
        private void LoadBots()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Chat Bots",
                Work = (w, e) =>
                {
                    var fetchXml = $@"
<fetch>
  <entity name='bot'>
    <attribute name='name' />
    <attribute name='botid' />
  </entity>
</fetch>";
                    var qe = new FetchExpression(fetchXml);


                    var botRecords = Service.RetrieveMultiple(qe);

                    e.Result = botRecords.Entities.Select(ent => new Bot() { Id = ent["botid"].ToString(), Name = ent["name"].ToString() }).ToList();

                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var returnBots = e.Result as List<Bot>;
                    if (returnBots.Any())
                    {
                        bots = returnBots;
                        cboBot.DataSource = bots;

                    }

                },
            });
        }
        private void LoadTopics()
        {
            if (cboBot.SelectedIndex == -1) return;
            string selectedBotId = ((Bot)cboBot.SelectedItem).Id;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Topics",
                Work = (w, e) =>
                {
                    var fetchXml = $@"
<fetch xmlns:generator='MarkMpn.SQL4CDS'>
  <entity name='botcomponent'>
    <attribute name='name' />
    <attribute name='category' />
    <attribute name='componentstate' />
    <attribute name='componenttype' />
    <attribute name='content' />
    <attribute name='description' />
    <attribute name='botcomponentid' />
    <filter>
      <condition attribute='componenttype' operator='eq' value='0'/>
    </filter>
    <link-entity name='bot_botcomponent' to='botcomponentid' from='botcomponentid' alias='Expr1' link-type='in'>
      <filter>
        <condition attribute='botid' operator='eq' value='{selectedBotId}'/>
      </filter>
    </link-entity>
  </entity>
</fetch>";
                    var qe = new FetchExpression(fetchXml);


                    var topicRecords = Service.RetrieveMultiple(qe);

                    e.Result = topicRecords.Entities.Select(ent => new Topic()
                    {
                        Id = ent["botcomponentid"].ToString(),
                        Name = ent["name"].ToString(),
                        Canvas = ent["content"].ToString(),
                        Description = ent.Attributes.Contains("description") ? ent["description"].ToString() : string.Empty
                    }).ToList();

                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var returnTopics = e.Result as List<Topic>;
                    if (returnTopics.Any())
                    {
                        gvTopics.DataSource = returnTopics;

                    }
                    ConfigGrid();
                },
            });
        }
        #endregion

        #region Other
        protected void ConfigGrid()
        {
            gvTopics.AutoResizeColumns();
            gvTopics.Columns["Name"].SortMode = DataGridViewColumnSortMode.Automatic;

        }
        #endregion
    }
}