﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using AIEToolProject.Source;

namespace AIEToolProject
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        /*
        * newButton_Click 
        * 
        * callback when the new button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void newButton_Click(object sender, EventArgs e)
        {
            //create a new EditorForm
            EditorForm newChild = new EditorForm();

            //set the parent
            newChild.MdiParent = this;

            //display the newly created child
            newChild.Show();

            //send the MDI window to the front of the screen
            newChild.Select();
        }


        /*
        * actionButton_Click 
        * 
        * callback when the action button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void actionButton_Click(object sender, EventArgs e)
        {
            EditorForm activeChild = (this.ActiveMdiChild as EditorForm);
            activeChild.spawnType = NodeType.ACTION;
        }


        /*
        * conditionButton_Click 
        * 
        * callback when the condition button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void conditionButton_Click(object sender, EventArgs e)
        {
            EditorForm activeChild = (this.ActiveMdiChild as EditorForm);
            activeChild.spawnType = NodeType.CONDITION;
        }


        /*
        * selectorButton_Click 
        * 
        * callback when the selector button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void selectorButton_Click(object sender, EventArgs e)
        {
            EditorForm activeChild = (this.ActiveMdiChild as EditorForm);
            activeChild.spawnType = NodeType.SELECTOR;
        }

        /*
        * sequenceButton_Click 
        * 
        * callback when the sequence button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void sequenceButton_Click(object sender, EventArgs e)
        {
            EditorForm activeChild = (this.ActiveMdiChild as EditorForm);
            activeChild.spawnType = NodeType.SEQUENCE;
        }


        /*
        * decoratorButton_Click 
        * 
        * callback when the decorator button is clicked
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void decoratorButton_Click(object sender, EventArgs e)
        {
            EditorForm activeChild = (this.ActiveMdiChild as EditorForm);
            activeChild.spawnType = NodeType.DECORATOR;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

        }
  

        /*
        * openButton_Click 
        * 
        * callback when the open button is clicked, opens an additional file dialog
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            //only allow XML files as input
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            //check that the open dialog opened properly
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //create a new EditorForm
                EditorForm newChild = new EditorForm();

                //only include the name of the file, not the extension
                newChild.Text = openFileDialog.SafeFileName.Split('.')[0];

                //set the parent
                newChild.MdiParent = this;

                //display the newly created child
                newChild.Show();

                //send the MDI window to the front of the screen
                newChild.Select();

                /*
                //create a de-serialiser object for the tree
                XmlSerializer serialiser = new XmlSerializer(typeof(BehaviourTree));

                //create a stream reader object for the xml reader
                StreamReader streamReader = new StreamReader(openFileDialog.FileName);

                //write the object to the stream
                newChild.snapshots.First = serialiser.Deserialize(streamReader) as BehaviourTree;

                newChild.snapshots.First.Relink();
                
                streamReader.Close();
                */
            }
        }


        /*
        * saveButton_Click 
        * 
        * callback when the save button is clicked, opens an additional file dialog
        * 
        * @param object sender - the object that sent the event
        * @param EventArgs e - description of the event
        * @returns void
        */
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //only save as an XML
            saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            //check that the save dialog opened properly
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                EditorForm activeChild = (this.ActiveMdiChild as EditorForm);

                //string splicing to get the desired format
                string fullPath = saveFileDialog.FileName;
                string[] splitPath = fullPath.Split('\\');
                string pathName = splitPath[splitPath.Length - 1];
                string name = pathName.Split('.')[0];

                //only include the name of the file, not the extension
                activeChild.Text = name;
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {

        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
