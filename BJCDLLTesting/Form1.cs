/* Title:           BJC Dll Testing
 * Date:            4-25-16
 * Author:          Terry Holmles
 *
 * Description:     This form is for testing */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VehiclesDLL;
using VehicleHistoryDLL;

namespace BJCDLLTesting
{
    public partial class Form1 : Form
    {
        //setting up the classes
        MessagesClass TheMessagesClass = new MessagesClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleHistoryClass TheVehicleHistoryClass = new VehicleHistoryClass();

        //setting up the data
        VehiclesDataSet TheVehiclesDataSet;
        VehicleHistoryDataSet TheVehicleHistoryDataSet;

        struct VehicleStructure
        {
            public int mintVehicleID;
            public int mintBJCNumber;
            public string mstrMake;
            public string mstrModel;
            public int mintEmployeeID;
        }

        VehicleStructure[] TheVehicles;
        int mintVehicleCounter;
        int mintVehicleUpperLimit;

        string mstrErrorMessage;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool blnFatalError = false;

            blnFatalError = LoadVehicleStructure();

            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("Mother Fucker, there is a problem");
            }
        }
        private bool LoadVehicleStructure()
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheVehiclesDataSet = TheVehicleClass.GetVehiclesInfo();

                intNumberOfRecords = TheVehiclesDataSet.vehicles.Rows.Count - 1;
                TheVehicles = new VehicleStructure[intNumberOfRecords + 1];
                mintVehicleCounter = 0;
                cboVehicleID.Items.Add("SELECT");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    TheVehicles[mintVehicleCounter].mintVehicleID = Convert.ToInt32(TheVehiclesDataSet.vehicles.Rows[intCounter][0]);
                    TheVehicles[mintVehicleCounter].mintBJCNumber = Convert.ToInt32(TheVehiclesDataSet.vehicles.Rows[intCounter][1]);
                    TheVehicles[mintVehicleCounter].mstrMake = Convert.ToString(TheVehiclesDataSet.vehicles.Rows[intCounter][4]).ToUpper();
                    TheVehicles[mintVehicleCounter].mstrModel = Convert.ToString(TheVehiclesDataSet.vehicles.Rows[intCounter][5]).ToUpper();
                    TheVehicles[mintVehicleCounter].mintEmployeeID = Convert.ToInt32(TheVehiclesDataSet.vehicles.Rows[intCounter][11]);
                    cboVehicleID.Items.Add(Convert.ToString(TheVehicles[mintVehicleCounter].mintVehicleID));
                    mintVehicleUpperLimit = mintVehicleCounter;
                    mintVehicleCounter++;
                }

                mintVehicleCounter = 0;
                cboVehicleID.SelectedIndex = 0;
            }
            catch(Exception Ex)
            {
                mstrErrorMessage = Ex.Message;

                blnFatalError = true;
            }

            return blnFatalError;
        }

        private void cboVehicleID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int intCounter;
            int intVehicleIDForSearch;

            if(cboVehicleID.Text != "SELECT")
            {
                intVehicleIDForSearch = Convert.ToInt32(cboVehicleID.Text);

                for(intCounter = 0; intCounter <= mintVehicleUpperLimit; intCounter++)
                {
                    if(intVehicleIDForSearch == TheVehicles[intCounter].mintVehicleID)
                    {
                        txtBJCNumber.Text = Convert.ToString(TheVehicles[intCounter].mintBJCNumber);
                        txtEmployeeID.Text = Convert.ToString(TheVehicles[intCounter].mintEmployeeID);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //setting local variables
            int intCounter;
            int intVehicleIDForSearch;
            string strInformation = "";

            try
            {
                intVehicleIDForSearch = Convert.ToInt32(cboVehicleID.Text);

                //loop
                for(intCounter = 0; intCounter <= mintVehicleUpperLimit; intCounter++)
                {
                    if (intVehicleIDForSearch == TheVehicles[intCounter].mintVehicleID)
                    {
                        TheVehiclesDataSet.vehicles.Rows[intCounter][11] = txtEmployeeID.Text;
                        TheVehicleClass.UpdateVehiclesDB(TheVehiclesDataSet);
                        strInformation = TheVehicleHistoryClass.CreateVehicleHistoryTransaction(intVehicleIDForSearch, 1211, 12, 12, "WHY ME", "FUCK");

                    }

                }

                TheMessagesClass.InformationMessage("It Is Fucking Done and " + strInformation);
            }
            catch(Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.Message);
            }
        }
    }
}
