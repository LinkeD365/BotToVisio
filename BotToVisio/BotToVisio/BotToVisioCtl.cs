using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.BotToVisio
{
    public partial class BotToVisioCtl : PluginControlBase, IGitHubPlugin, IPayPalPlugin
    {
        #region Interface bits
        public string RepositoryName => "BotToVisio";
        public string UserName => "LinkeD365";

        public string DonationDescription => "PVA to Visio Fans";

        public string EmailAccount => "carl.cookson@gmail.com";
        #endregion
        #region Ctrl Load

        private Settings mySettings;

        internal List<Bot> bots { get; private set; }

        public BotToVisioCtl()
        {
            InitializeComponent();
            Utils.Ai.WriteEvent("Control Loaded");
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
            ExecuteMethod(LoadEntities);
        }


        #endregion

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        #region Events
        private void cboBot_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(LoadTopics);
        }

        private void btnCreateVisio_Click(object sender, EventArgs e)
        {
            var selectedTopics = new List<Topic>();

            if (gvTopics.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one Topic to document", "Select Topic(s)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int topicCount = 1;
            Utils.ActionCount = 0;
            SaveFileDialog saveDialog;
            if (gvTopics.SelectedRows.Count == 1)
            {
                var selectedTopic = (Topic)gvTopics.SelectedRows[0].DataBoundItem;
                saveDialog = GetSaveDialog(selectedTopic.Name);
                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Utils.CreateVisio(selectedTopic, saveDialog.FileName, 1);
                Utils.CompleteVisio(saveDialog.FileName);
            }
            else
            {
                saveDialog = GetSaveDialog(((Bot)cboBot.SelectedItem).Name + ".vsdx");
                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (DataGridViewRow row in gvTopics.SelectedRows)
                {
                    var selectedTopic = (Topic)row.DataBoundItem;
                    Utils.CreateVisio(selectedTopic, saveDialog.FileName, topicCount);
                    topicCount++;
                }
                Utils.CompleteVisio(saveDialog.FileName);

            }

            Utils.Ai.WriteEvent("Topics Created", topicCount);
            Utils.Ai.WriteEvent("Actions Created", Utils.ActionCount);

            if (MessageBox.Show($@"{topicCount} topic{( topicCount > 1 ? "s have":" has")} been created with {Utils.ActionCount} actions.
Do you want to open the Visio File?", "Visio created successfully", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(saveDialog.FileName);
            }
        }



        #endregion

        #region Bot retrieval
        private void LoadEntities()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Custom Entities",
                Work = (w, e) =>
                {
                    var fetchXml = $@"
<fetch xmlns:generator='MarkMpn.SQL4CDS'>
  <entity name='botcomponent'>
    <attribute name='name' />
    <attribute name='content' />
    
    <filter>
      <condition attribute='componenttype' operator='eq' value='3' />
    </filter>
  </entity>
</fetch>";
                    var qe = new FetchExpression(fetchXml);


                    var neRecords = Service.RetrieveMultiple(qe);
                    var jNEs = neRecords.Entities.Where(ent => ent["content"].ToString().Contains("namedEntities"))
                           .Select(ne => JObject.Parse(ne["content"].ToString()));
                    e.Result = jNEs.Select(jobj => new CustomType((JObject)jobj)).ToList();
                    //   e.Result = botRecords.Entities.Select(ent => new Bot() { Id = ent["botid"].ToString(), Name = ent["name"].ToString() }).ToList();

                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var customTypes = e.Result as List<CustomType>;
                    if (customTypes.Any())
                    {
                        Utils.CustomTypes = customTypes;
                    }
                    else Utils.CustomTypes = new List<CustomType> { };
                },
            });
        }

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
    <attribute name='schemaname' />
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
                        Description = ent.Attributes.Contains("description") ? ent["description"].ToString() : string.Empty,
                        SchemaName = ent.Attributes.Contains("schemaname") ? ent["schemaname"].ToString() : string.Empty
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
                        Utils.Topics = returnTopics;
                        gvTopics.DataSource = returnTopics;

                    }
                    else
                    {
                        Utils.Topics = new List<Topic>();
                        gvTopics.DataSource = null;
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

        private SaveFileDialog GetSaveDialog(string fileName)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Visio Files(*.vsdx) | *.vsdx";
            saveFileDialog.DefaultExt = "vsdx";

            saveFileDialog.FileName = fileName;
            return saveFileDialog;
        }
        #endregion


    }
}