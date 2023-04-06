using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Collections;
using System.Reflection;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WorldCupSimulator
{
    public partial class Form1 : Form
    {
        //Variables used throughout the entire app
        List<Country> participants = new List<Country>();
        int whatRoundIsIt = 0;
        string[] orderOfButtons = new string[4];
        bool isItPlayOff = false;
        CountryWithStats[] playOffArray = new CountryWithStats[16];
        CountryWithStats[] quarterfinalArray = new CountryWithStats[8];
        CountryWithStats[] semifinalArray = new CountryWithStats[4];
        CountryWithStats[] finalArray = new CountryWithStats[4];
        CountryWithStats[] resultsArray = new CountryWithStats[3];
        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAddFromQatar2022_Click(object sender, EventArgs e)
        {
            if (participants.Count > 0) //Prevents the amount of participants from surpassing 32
            {
                MessageBox.Show("You already have teams in your setup! If you wish to delete them, use the \"Clear all\" button first.");
            }
            else
            {
                //There is probably a better way to do this, however i sadly do not know it.
                //The only other solution i came up with was making this a separate .csv,
                //which would however only move this wall of text to a different file...
                participants.Add(new Country("Argentina", "LightBlue", "DarkBlue", "X"));
                participants.Add(new Country("Australia", "Yellow", "DarkBlue", "X"));
                participants.Add(new Country("Belgium", "Red", "White", "X"));
                participants.Add(new Country("Brazil", "Yellow", "DarkBlue", "X"));
                participants.Add(new Country("Cameroon", "Green", "White", "X"));
                participants.Add(new Country("Canada", "Red", "White", "X"));
                participants.Add(new Country("Costa Rica", "Red", "White", "X"));
                participants.Add(new Country("Croatia", "White", "DarkBlue", "X"));
                participants.Add(new Country("Denmark", "Red", "White", "X"));
                participants.Add(new Country("Ecuador", "Yellow", "DarkBlue", "X"));
                participants.Add(new Country("England", "White", "Red", "X"));
                participants.Add(new Country("France", "DarkBlue", "White", "X"));
                participants.Add(new Country("Germany", "White", "Black", "X"));
                participants.Add(new Country("Ghana", "White", "Red", "X"));
                participants.Add(new Country("Iran", "White", "Red", "X"));
                participants.Add(new Country("Japan", "Blue", "White", "X"));
                participants.Add(new Country("Mexico", "Green", "White", "X"));
                participants.Add(new Country("Morocco", "Red", "White", "X"));
                participants.Add(new Country("Netherlands", "Orange", "DarkBlue", "X"));
                participants.Add(new Country("Poland", "White", "Red", "X"));
                participants.Add(new Country("Portugal", "Red", "White", "X"));
                participants.Add(new Country("Qatar", "Maroon", "White", "X"));
                participants.Add(new Country("Saudi Arabia", "White", "Green", "X"));
                participants.Add(new Country("Senegal", "White", "Green", "X"));
                participants.Add(new Country("Serbia", "Red", "White", "X"));
                participants.Add(new Country("South Korea", "Red", "Black", "X"));
                participants.Add(new Country("Spain", "Red", "LightBlue", "X"));
                participants.Add(new Country("Switzerland", "Red", "White", "X"));
                participants.Add(new Country("Tunisia", "Red", "White", "X"));
                participants.Add(new Country("USA", "White", "LightBlue", "X"));
                participants.Add(new Country("Uruguay", "LightBlue", "White", "X"));
                participants.Add(new Country("Wales", "Red", "White", "X"));

                //Adds the name of each country to listOfParticipants
                foreach (Country c in participants)
                {
                    listOfParticipants.Items.Add(c.Name);
                }
                UpdateCountryCount(); //Updates the displayed country count. A method due to it being used several times
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (listOfParticipants.SelectedIndex != -1)
            {
                //This first removes the country from the List called participants, then from the array
                participants.RemoveAll(x => x.Name == listOfParticipants.SelectedItem.ToString());
                listOfParticipants.Sorted = true;
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                UpdateCountryCount();
            }
            else
            {
                //Crash prevention with info for the user
                MessageBox.Show("You can only delete countries from the List of Participants! If you wish to delete a country from a group, move it back into the List of Participants first.");
            }
        }

        public void UpdateCountryCount()
        {
            //The amount of participants is determined by simply counting the number of entries in the List
            int amountOfCountries = participants.Count;
            labelAmountOfCountries.Text = "Teams: " + amountOfCountries + " / 32";
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            //Clears the entire setup
            participants.Clear();
            listBoxA.Items.Clear();
            listBoxB.Items.Clear();
            listBoxC.Items.Clear();
            listBoxD.Items.Clear();
            listBoxE.Items.Clear();
            listBoxF.Items.Clear();
            listBoxG.Items.Clear();
            listBoxH.Items.Clear();
            listOfParticipants.Items.Clear();
            UpdateCountryCount();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //This button exports the current configuration into a .csv file
            SaveFileDialog export = new SaveFileDialog() { Filter = "CSV|*.csv", ValidateNames = true };
            string fileName = "";
            if (export.ShowDialog() == DialogResult.OK) //pop-up window that lets the user select a place to save the file
            {
                fileName = export.FileName;
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Append);
                using (StreamWriter strmWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    //This isn't a particularly safe way to prevent the user from inserting .csv's not originally exported from this app,
                    //however i believe that it's enough, as the user would have to edit other files themselves.
                    strmWriter.WriteLine("ThisIsInFactACorrectFile");
                    foreach (Country c in participants)
                    {
                        strmWriter.WriteLine(string.Format("{0},{1},{2},{3}", c.Name, c.Colormain, c.Colorsecondary, c.Group));
                    }
                    strmWriter.Close();
                }
            }                
        }

        private void btnImport_Click(object sender, EventArgs e)
        {            
            OpenFileDialog import = new OpenFileDialog();
            string fileName = "";
            if (import.ShowDialog() == DialogResult.OK)
            {
                fileName = import.FileName;
            }
            
            if (!string.IsNullOrEmpty(fileName))
            {
                if (Path.GetExtension(fileName) != ".csv")
                {
                    MessageBox.Show("The selected file isn't csv.");
                    return;
                }
                participants.Clear();
                listOfParticipants.Items.Clear();
                //This gets the values from the .csv file and converts them into List items
                using (StreamReader strmReader = new StreamReader(fileName))
                {
                    string firstLine = strmReader.ReadLine();
                    string singleLineFromImport = "";
                    if (firstLine == "ThisIsInFactACorrectFile") //Checks if the .csv was made by this app
                    {
                        while ((singleLineFromImport = strmReader.ReadLine()) != null)
                        {
                            string[] x = singleLineFromImport.Split(',');
                            participants.Add(new Country(x[0], x[1], x[2], x[3]));
                        }
                    }   
                    else
                    {
                        MessageBox.Show("This isn't a .csv file originally exported from this WPF!");
                        return;
                    }
                }
                foreach (Country c in participants)
                {
                    listOfParticipants.Items.Add(c.Name);
                }
                listOfParticipants.Sorted = true;
                UpdateCountryCount();
            }                
        }

        private void btnAddCustomTeam_Click(object sender, EventArgs e)
        {
            string customCountryName = "";
            string primaryCustomColor = "";
            string secondaryCustomColor = "";

            //To prevent multiple messageboxes from popping up
            bool somethingPoppedUpAlready = false;

            //This checks for teams with matching names
            bool matchingCountryFound = false;
            foreach (Country c in participants)
            {
                if (c.Name == textBoxCustomName.Text)
                {
                    matchingCountryFound = true;
                }
            }

            if (string.IsNullOrEmpty(textBoxCustomName.Text))
            {
                MessageBox.Show("You need to set a name for your custom team!");
                somethingPoppedUpAlready = true;
            }
            else if (matchingCountryFound == true)
            {
                MessageBox.Show("A team with that exact name is already in the list of participants!");
                somethingPoppedUpAlready = true;
            }
            else
            {
                customCountryName = textBoxCustomName.Text;
            }

            //Once again, this probably has a much more elegant solution, but i don't know it.
            //It would've been a good idea to put this into a method, however this is the only place
            //in the program where it is used.
            if (rbpWhite.Checked)
                primaryCustomColor = "White";
            else if (rbpBlack.Checked)
                primaryCustomColor = "Black";
            else if (rbpGray.Checked)
                primaryCustomColor = "Gray";
            else if (rbpBrown.Checked)
                primaryCustomColor = "Brown";
            else if (rbpRed.Checked)
                primaryCustomColor = "Red";
            else if (rbpMaroon.Checked)
                primaryCustomColor = "Maroon";
            else if (rbpYellow.Checked)
                primaryCustomColor = "Yellow";
            else if (rbpOrange.Checked)
                primaryCustomColor = "Orange";
            else if (rbpGreen.Checked)
                primaryCustomColor = "Green";
            else if (rbpLime.Checked)
                primaryCustomColor = "Lime";
            else if (rbpLightBlue.Checked)
                primaryCustomColor = "LightBlue";
            else if (rbpDarkBlue.Checked)
                primaryCustomColor = "DarkBlue";
            else
            {
                if (somethingPoppedUpAlready == false)
                    MessageBox.Show("No primary color selected!");

                somethingPoppedUpAlready = true;
            }

            if (rbsWhite.Checked)
                secondaryCustomColor = "White";
            else if (rbsBlack.Checked)
                secondaryCustomColor = "Black";
            else if (rbsGray.Checked)
                secondaryCustomColor = "Gray";
            else if (rbsBrown.Checked)
                secondaryCustomColor = "Brown";
            else if (rbsRed.Checked)
                secondaryCustomColor = "Red";
            else if (rbsMaroon.Checked)
                secondaryCustomColor = "Maroon";
            else if (rbsYellow.Checked)
                secondaryCustomColor = "Yellow";
            else if (rbsOrange.Checked)
                secondaryCustomColor = "Orange";
            else if (rbsGreen.Checked)
                secondaryCustomColor = "Green";
            else if (rbsLime.Checked)
                secondaryCustomColor = "Lime";
            else if (rbsLightBlue.Checked)
                secondaryCustomColor = "LightBlue";
            else if (rbsDarkBlue.Checked)
                secondaryCustomColor = "DarkBlue";
            else
            {
                if (somethingPoppedUpAlready == false)
                    MessageBox.Show("No secondary color selected!");

                somethingPoppedUpAlready = true;
            }

            if ((primaryCustomColor != secondaryCustomColor) && primaryCustomColor != "" && secondaryCustomColor != "" && matchingCountryFound == false)
            {
                if (participants.Count < 32)
                {
                    //This adds the custom team to participants and resets the UI controls
                    participants.Add(new Country(textBoxCustomName.Text, primaryCustomColor, secondaryCustomColor, "X"));
                    listOfParticipants.Items.Add(textBoxCustomName.Text);
                    listOfParticipants.Sorted = true;
                    UpdateCountryCount();
                    textBoxCustomName.Clear();
                    rbpWhite.Checked = false; rbpBlack.Checked = false; rbpGray.Checked = false; rbpBrown.Checked = false;
                    rbpRed.Checked = false; rbpMaroon.Checked = false; rbpYellow.Checked = false; rbpOrange.Checked = false;
                    rbpGreen.Checked = false; rbpLime.Checked = false; rbpLightBlue.Checked = false; rbpDarkBlue.Checked = false;
                    rbsWhite.Checked = false; rbsBlack.Checked = false; rbsGray.Checked = false; rbsBrown.Checked = false;
                    rbsRed.Checked = false; rbsMaroon.Checked = false; rbsYellow.Checked = false; rbsOrange.Checked = false;
                    rbsGreen.Checked = false; rbsLime.Checked = false; rbsLightBlue.Checked = false; rbsDarkBlue.Checked = false;
                }
                else if (somethingPoppedUpAlready == false)
                {
                    MessageBox.Show("You can only have 32 teams! Please, remove one before adding another.");
                }
            }
            else if (somethingPoppedUpAlready == false)
            {
                MessageBox.Show("The primary and secondary colors cannot match!");
            }
        }

        //Each button is the same except for UI control names, however i sadly haven't figured out how to make them into a method...
        private void btnAddA_Click(object sender, EventArgs e)
        {
            if (listBoxA.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country a in participants)
                {
                    if (a.Name == selector)
                    {
                        a.Group = "A";
                    }
                }
                listBoxA.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxA.Sorted = true;
                ClearSelects("A");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddB_Click(object sender, EventArgs e)
        {
            if (listBoxB.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country b in participants)
                {
                    if (b.Name == selector)
                    {
                        b.Group = "B";
                    }
                }
                listBoxB.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxB.Sorted = true;
                ClearSelects("B");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddC_Click(object sender, EventArgs e)
        {
            if (listBoxC.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country c in participants)
                {
                    if (c.Name == selector)
                    {
                        c.Group = "C";
                    }
                }
                listBoxC.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxC.Sorted = true;
                ClearSelects("C");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddD_Click(object sender, EventArgs e)
        {
            if (listBoxD.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country d in participants)
                {
                    if (d.Name == selector)
                    {
                        d.Group = "D";
                    }
                }
                listBoxD.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxD.Sorted = true;
                ClearSelects("D");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddE_Click(object sender, EventArgs e)
        {
            if (listBoxE.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country country_e in participants)
                {
                    if (country_e.Name == selector)
                    {
                        country_e.Group = "E";
                    }
                }
                listBoxE.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxE.Sorted = true;
                ClearSelects("E");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddF_Click(object sender, EventArgs e)
        {
            if (listBoxF.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country f in participants)
                {
                    if (f.Name == selector)
                    {
                        f.Group = "F";
                    }
                }
                listBoxF.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxF.Sorted = true;
                ClearSelects("F");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddG_Click(object sender, EventArgs e)
        {
            if (listBoxG.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country g in participants)
                {
                    if (g.Name == selector)
                    {
                        g.Group = "G";
                    }
                }
                listBoxG.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxG.Sorted = true;
                ClearSelects("G");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnAddH_Click(object sender, EventArgs e)
        {
            if (listBoxH.Items.Count < 4 && listOfParticipants.SelectedIndex != -1)
            {
                string selector = listOfParticipants.SelectedItem.ToString();
                foreach (Country h in participants)
                {
                    if (h.Name == selector)
                    {
                        h.Group = "H";
                    }
                }
                listBoxH.Items.Add(selector);
                listOfParticipants.Items.Remove(listOfParticipants.SelectedItem);
                listBoxH.Sorted = true;
                ClearSelects("H");
            }
            else
                MessageBox.Show("You either tried to use this button without selecting anything from Participants, or you have more than 4 teams in your target group.");
        }

        private void btnUngroup_Click(object sender, EventArgs e)
        {
            string selector = "";
            string whatListBox = "";        
            if (listBoxA.SelectedIndex != -1)
            {
                selector = listBoxA.SelectedItem.ToString();
                whatListBox = "A";
            }
            else if (listBoxB.SelectedIndex != -1)
            {
                selector = listBoxB.SelectedItem.ToString();
                whatListBox = "B";
            }
            else if (listBoxC.SelectedIndex != -1)
            {
                selector = listBoxC.SelectedItem.ToString();
                whatListBox = "C";
            }
            else if (listBoxD.SelectedIndex != -1)
            {
                selector = listBoxD.SelectedItem.ToString();
                whatListBox = "D";
            }
            else if (listBoxE.SelectedIndex != -1)
            {
                selector = listBoxE.SelectedItem.ToString();
                whatListBox = "E";
            }
            else if (listBoxF.SelectedIndex != -1)
            {
                selector = listBoxF.SelectedItem.ToString();
                whatListBox = "F";
            }
            else if (listBoxG.SelectedIndex != -1)
            {
                selector = listBoxG.SelectedItem.ToString();
                whatListBox = "G";
            }
            else if (listBoxH.SelectedIndex != -1)
            {
                selector = listBoxH.SelectedItem.ToString();
                whatListBox = "H";
            }
            else
                MessageBox.Show("You haven't selected any team in any of the groups!");

            foreach (Country x in participants)
            {
                if (x.Name == selector)
                {
                    x.Group = "X";
                }
            }

            if (selector != "")
                listOfParticipants.Items.Add(selector);

            switch (whatListBox)
            {
                case "A":
                    listBoxA.Items.Remove(selector);
                    break;
                case "B":
                    listBoxB.Items.Remove(selector);
                    break;
                case "C":
                    listBoxC.Items.Remove(selector);
                    break;
                case "D":
                    listBoxD.Items.Remove(selector);
                    break;
                case "E":
                    listBoxE.Items.Remove(selector);
                    break;
                case "F":
                    listBoxF.Items.Remove(selector);
                    break;
                case "G":
                    listBoxG.Items.Remove(selector);
                    break;
                case "H":
                    listBoxH.Items.Remove(selector);
                    break;
                default:
                    break;
            }
            listOfParticipants.Sorted = true;
            ClearSelects("X");
        }

        //I experienced several bugs with double selects, so the following methods were created to prevent them
        public void ClearSelects(string x)
        {
            if (x != "A")
                listBoxA.ClearSelected();
            if (x != "B")
                listBoxB.ClearSelected();
            if (x != "C")
                listBoxC.ClearSelected();
            if (x != "D")
                listBoxD.ClearSelected();
            if (x != "E")
                listBoxE.ClearSelected();
            if (x != "F")
                listBoxF.ClearSelected();
            if (x != "G")
                listBoxG.ClearSelected();
            if (x != "H")
                listBoxH.ClearSelected();
            if (x != "X")
                listOfParticipants.ClearSelected();
        }

        private void listOfParticipants_Click(object sender, EventArgs e)
        {
            ClearSelects("X");
        }

        private void listBoxA_Click(object sender, EventArgs e)
        {
            ClearSelects("A");
        }

        private void listBoxB_Click(object sender, EventArgs e)
        {
            ClearSelects("B");
        }

        private void listBoxC_Click(object sender, EventArgs e)
        {
            ClearSelects("C");
        }

        private void listBoxD_Click(object sender, EventArgs e)
        {
            ClearSelects("D");
        }

        private void listBoxE_Click(object sender, EventArgs e)
        {
            ClearSelects("E");
        }

        private void listBoxF_Click(object sender, EventArgs e)
        {
            ClearSelects("F");
        }

        private void listBoxG_Click(object sender, EventArgs e)
        {
            ClearSelects("G");
        }

        private void listBoxH_Click(object sender, EventArgs e)
        {
            ClearSelects("H");
        }

        //Randomizes all remaining teams in listOfParticipants into groups while ensuring that all the groups don't end up with over 4 teams.
        private void btnAddRest_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            foreach (Country c in participants)
            {
                bool successfullySorted = false;
                if (c.Group == "X")
                {
                    int groupRandomizer = random.Next(1, 9);
                    do
                    {
                        switch (groupRandomizer)
                        {
                            case 1:
                                if (listBoxA.Items.Count < 4)
                                {
                                    c.Group = "A";
                                    listBoxA.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxA.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    //If a group already has 4 teams, this makes the code look again in the next group.
                                    //Because we are ensuring that only 32 teams can exist at once, this will always find a group
                                    //with an availible spot.
                                    groupRandomizer++;
                                }
                                break;
                            case 2:
                                if (listBoxB.Items.Count < 4)
                                {
                                    c.Group = "B";
                                    listBoxB.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxB.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 3:
                                if (listBoxC.Items.Count < 4)
                                {
                                    c.Group = "C";
                                    listBoxC.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxC.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 4:
                                if (listBoxD.Items.Count < 4)
                                {
                                    c.Group = "D";
                                    listBoxD.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxD.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 5:
                                if (listBoxE.Items.Count < 4)
                                {
                                    c.Group = "E";
                                    listBoxE.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxE.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 6:
                                if (listBoxF.Items.Count < 4)
                                {
                                    c.Group = "F";
                                    listBoxF.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxF.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 7:
                                if (listBoxG.Items.Count < 4)
                                {
                                    c.Group = "G";
                                    listBoxG.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxG.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer++;
                                }
                                break;
                            case 8:
                                if (listBoxH.Items.Count < 4)
                                {
                                    c.Group = "H";
                                    listBoxH.Items.Add(c.Name);
                                    listOfParticipants.Items.Remove(c.Name);
                                    listBoxH.Sorted = true;
                                    successfullySorted = true;
                                }
                                else
                                {
                                    groupRandomizer = 1;
                                }
                                break;
                        }
                    } while (successfullySorted == false);
                }
            }
        }

        //An issue with this app and its code is that for most of its run time there are 8 identical programs - groups - at the same time.
        //Due to that most of the code here has to repeat 8 times.
        //I'd like to get back to this app in the future to see if a better way exists, however it is April already and i'd like to have
        //it done before the Maturita season begins.
        List<CountryWithStats> list_GroupA = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupB = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupC = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupD = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupE = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupF = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupG = new List<CountryWithStats>();
        List<CountryWithStats> list_GroupH = new List<CountryWithStats>();

        private void btnFinishSetup_Click(object sender, EventArgs e)
        {
            //"If" conditional to ensure there are exactly 32 teams and all of them are sorted.
            if (listBoxA.Items.Count == 4 && listBoxB.Items.Count == 4 && listBoxC.Items.Count == 4 && listBoxD.Items.Count == 4 && listBoxE.Items.Count == 4 && listBoxF.Items.Count == 4 && listBoxG.Items.Count == 4 && listBoxH.Items.Count == 4 && listOfParticipants.Items.Count == 0 && participants.Count == 32)
            {
                foreach (Country c in participants)
                {
                    switch (c.Group)
                    {
                        case "A":
                            list_GroupA.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupA.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "B":
                            list_GroupB.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupB.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "C":
                            list_GroupC.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupC.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "D":
                            list_GroupD.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupD.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "E":
                            list_GroupE.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupE.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "F":
                            list_GroupF.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupF.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "G":
                            list_GroupG.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupG.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        case "H":
                            list_GroupH.Add(new CountryWithStats(c.Name, c.Colormain, c.Colorsecondary, c.Group, (list_GroupH.Count + 1), 0, 0, 0, 0, 0));
                            break;
                        default:
                            MessageBox.Show("If you see this, something went very wrong :(");
                            break;
                    }
                }
                //A rather unsightly wall of commands to disable/enable various UI controls on the PARTICIPANTS and other tabs.
                //While coding this app, i wanted to use UI control disabling as a way to prevent bugs and to use them to
                //show the user what are they currently supposed to be doing - pressing.
                //The btnExport button is the only one availible throughout the entire runtime - The user could've spent a long
                //time creating custom teams, so it would be unfair to delete it all just because they forgot to export right away.
                btnAddFromQatar2022.Enabled = false;
                btnDeleteSelected.Enabled = false;
                btnAddCustomTeam.Enabled = false;
                textBoxCustomName.Enabled = false;
                rbpWhite.Enabled = false; rbpBlack.Enabled = false; rbpGray.Enabled = false; rbpBrown.Enabled = false;
                rbpRed.Enabled = false; rbpMaroon.Enabled = false; rbpYellow.Enabled = false; rbpOrange.Enabled = false;
                rbpGreen.Enabled = false; rbpLime.Enabled = false; rbpLightBlue.Enabled = false; rbpDarkBlue.Enabled = false;
                rbsWhite.Enabled = false; rbsBlack.Enabled = false; rbsGray.Enabled = false; rbsBrown.Enabled = false;
                rbsRed.Enabled = false; rbsMaroon.Enabled = false; rbsYellow.Enabled = false; rbsOrange.Enabled = false;
                rbsGreen.Enabled = false; rbsLime.Enabled = false; rbsLightBlue.Enabled = false; rbsDarkBlue.Enabled = false;
                rbpWhite.Enabled = false;
                btnClearAll.Enabled = false;
                btnImport.Enabled= false;
                btnAddA.Enabled = false; btnAddB.Enabled = false; btnAddC.Enabled = false; btnAddD.Enabled = false;
                btnAddE.Enabled = false; btnAddF.Enabled = false; btnAddG.Enabled = false; btnAddH.Enabled = false;
                btnUngroup.Enabled = false;
                btnAddRest.Enabled = false;    
                listBoxA.Enabled = false; listBoxB.Enabled = false; listBoxC.Enabled = false; listBoxD.Enabled = false;
                listBoxE.Enabled = false; listBoxF.Enabled = false; listBoxG.Enabled = false; listBoxH.Enabled = false;
                btnStartRounds.Enabled = true;
                tabControl1.SelectedIndex = 2;
                insertIntoGROUPS();
                whatRoundIsIt = 1;
                btnStartRounds.Text = "Begin Round 1";
                btnStartRounds.Focus();
                btnFinishSetup.Enabled = false;
            }
            else
            {                
                MessageBox.Show("Your current setup isn't correct! To be able to finish the setup, you need to have 32 teams in total, 4 teams in each group and no teams in the \"Participants:\" box.");
            }
        }

        private void btnMoveToParticipants_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        public void insertIntoGROUPS() //Inserts into dataGridViews on the GROUPS tab.
        {
            dgw_A.Rows.Clear();
            dgw_B.Rows.Clear();
            dgw_C.Rows.Clear();
            dgw_D.Rows.Clear();
            dgw_E.Rows.Clear();
            dgw_F.Rows.Clear();
            dgw_G.Rows.Clear();
            dgw_H.Rows.Clear();

            foreach (CountryWithStats c in list_GroupA)
            {
                dgw_A.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupB)
            {
                dgw_B.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupC)
            {
                dgw_C.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupD)
            {
                dgw_D.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupE)
            {
                dgw_E.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupF)
            {
                dgw_F.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupG)
            {
                dgw_G.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
            foreach (CountryWithStats c in list_GroupH)
            {
                dgw_H.Rows.Add(c.Name, c.Wins, c.Draws, c.Losses, c.Goals, c.Points);
            }
        }        

        List<int> listOfEventsForARound;
        int hMRP_Inserter = 0;
        //Code here randomizes a list of matches in a round and sends it to the RoundX methods based on what round is currently happening.
        //If the Play-Off is about to start, it instead inserts the teams into their listBoxes on the PLAY-OFF tab.
        private void btnStartRounds_Click(object sender, EventArgs e)
        {
            textBoxTeam1.Enabled = true; 
            textBoxTeam2.Enabled = true;
            btnSubmitAndContinue.Enabled = true;
            btnStartRounds.Enabled = false;
            if (whatRoundIsIt != 0)
            {
                hMRP_Inserter = 0;
                tabControl1.SelectedIndex = 3;
                listOfEventsForARound = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                ListShuffler.Shuffle(listOfEventsForARound);
                switch (whatRoundIsIt)
                {
                    case 1:
                        RoundOne(listOfEventsForARound, hMRP_Inserter);
                        break;
                    case 2:
                        RoundTwo(listOfEventsForARound, hMRP_Inserter);
                        break;
                    case 3:
                        RoundThree(listOfEventsForARound, hMRP_Inserter);
                        break;
                    case 4:
                        isItPlayOff = true;
                        ros1.Items.Add(playOffArray[0].Name);
                        ros1.Items.Add(playOffArray[1].Name);
                        ros2.Items.Add(playOffArray[2].Name);
                        ros2.Items.Add(playOffArray[3].Name);
                        ros3.Items.Add(playOffArray[4].Name);
                        ros3.Items.Add(playOffArray[5].Name);
                        ros4.Items.Add(playOffArray[6].Name);
                        ros4.Items.Add(playOffArray[7].Name);
                        ros5.Items.Add(playOffArray[8].Name);
                        ros5.Items.Add(playOffArray[9].Name);
                        ros6.Items.Add(playOffArray[10].Name);
                        ros6.Items.Add(playOffArray[11].Name);
                        ros7.Items.Add(playOffArray[12].Name);
                        ros7.Items.Add(playOffArray[13].Name);
                        ros8.Items.Add(playOffArray[14].Name);
                        ros8.Items.Add(playOffArray[15].Name);
                        btnStartRounds.Enabled = false;
                        btnStartPlayOffRounds.Enabled = true;
                        textBoxTeam1.Enabled = false;
                        textBoxTeam2.Enabled = false;
                        btnSubmitAndContinue.Enabled = false;
                        tabControl1.SelectedIndex = 5;
                        btnStartPlayOffRounds.Focus();
                        btnStartRounds.Text = "x";
                        break;
                }
            }
        }

        //This isn't my code, it's a "Fisher-Yates shuffle"
        //I found it while looking for a way to shuffle a list, and i wouldn't be able to create a better algorithm myself.
        public static class ListShuffler
        {
            private static Random rng = new Random();
            public static void Shuffle<T>(IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }

        //During each round, matches in a group are predetermined. What is randomized is their order.
        public void RoundOne(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForARound[howManyRoundsPlayed])
            {
                case 1:
                    SetupAMatch("A", 0, 1);
                    break;
                case 2:
                    SetupAMatch("A", 2, 3);
                    break;
                case 3:
                    SetupAMatch("B", 0, 1);
                    break;
                case 4:
                    SetupAMatch("B", 2, 3);
                    break;
                case 5:
                    SetupAMatch("C", 0, 1);
                    break;
                case 6:
                    SetupAMatch("C", 2, 3);
                    break;
                case 7:
                    SetupAMatch("D", 0, 1);
                    break;
                case 8:
                    SetupAMatch("D", 2, 3);
                    break;
                case 9:
                    SetupAMatch("E", 0, 1);
                    break;
                case 10:
                    SetupAMatch("E", 2, 3);
                    break;
                case 11:
                    SetupAMatch("F", 0, 1);
                    break;
                case 12:
                    SetupAMatch("F", 2, 3);
                    break;
                case 13:
                    SetupAMatch("G", 0, 1);
                    break;
                case 14:
                    SetupAMatch("G", 2, 3);
                    break;
                case 15:
                    SetupAMatch("H", 0, 1);
                    break;
                case 16:
                    SetupAMatch("H", 2, 3);
                    break;
            }
        }

        public void RoundTwo(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForARound[howManyRoundsPlayed])
            {
                case 1:
                    SetupAMatch("A", 0, 2);
                    break;
                case 2:
                    SetupAMatch("A", 1, 3);
                    break;
                case 3:
                    SetupAMatch("B", 0, 2);
                    break;
                case 4:
                    SetupAMatch("B", 1, 3);
                    break;
                case 5:
                    SetupAMatch("C", 0, 2);
                    break;
                case 6:
                    SetupAMatch("C", 1, 3);
                    break;
                case 7:
                    SetupAMatch("D", 0, 2);
                    break;
                case 8:
                    SetupAMatch("D", 1, 3);
                    break;
                case 9:
                    SetupAMatch("E", 0, 2);
                    break;
                case 10:
                    SetupAMatch("E", 1, 3);
                    break;
                case 11:
                    SetupAMatch("F", 0, 2);
                    break;
                case 12:
                    SetupAMatch("F", 1, 3);
                    break;
                case 13:
                    SetupAMatch("G", 0, 2);
                    break;
                case 14:
                    SetupAMatch("G", 1, 3);
                    break;
                case 15:
                    SetupAMatch("H", 0, 2);
                    break;
                case 16:
                    SetupAMatch("H", 1, 3);
                    break;
            }
        }

        public void RoundThree(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForARound[howManyRoundsPlayed])
            {
                case 1:
                    SetupAMatch("A", 0, 3);
                    break;
                case 2:
                    SetupAMatch("A", 1, 2);
                    break;
                case 3:
                    SetupAMatch("B", 0, 3);
                    break;
                case 4:
                    SetupAMatch("B", 1, 2);
                    break;
                case 5:
                    SetupAMatch("C", 0, 3);
                    break;
                case 6:
                    SetupAMatch("C", 1, 2);
                    break;
                case 7:
                    SetupAMatch("D", 0, 3);
                    break;
                case 8:
                    SetupAMatch("D", 1, 2);
                    break;
                case 9:
                    SetupAMatch("E", 0, 3);
                    break;
                case 10:
                    SetupAMatch("E", 1, 2);
                    break;
                case 11:
                    SetupAMatch("F", 0, 3);
                    break;
                case 12:
                    SetupAMatch("F", 1, 2);
                    break;
                case 13:
                    SetupAMatch("G", 0, 3);
                    break;
                case 14:
                    SetupAMatch("G", 1, 2);
                    break;
                case 15:
                    SetupAMatch("H", 0, 3);
                    break;
                case 16:
                    SetupAMatch("H", 1, 2);
                    break;
            }
        }
        public void PlayOffRoundOf16(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForRoundOf16[howManyRoundsPlayed])
            {
                case 1:
                    SetupAPlayOffMatch("Round-of-16", "1", 0, 1);
                    break;
                case 2:
                    SetupAPlayOffMatch("Round-of-16", "2", 2, 3);
                    break;
                case 3:
                    SetupAPlayOffMatch("Round-of-16", "3", 4, 5);
                    break;
                case 4:
                    SetupAPlayOffMatch("Round-of-16", "4", 6, 7);
                    break;
                case 5:
                    SetupAPlayOffMatch("Round-of-16", "5", 8, 9);
                    break;
                case 6:
                    SetupAPlayOffMatch("Round-of-16", "6", 10, 11);
                    break;
                case 7:
                    SetupAPlayOffMatch("Round-of-16", "7", 12, 13);
                    break;
                case 8:
                    SetupAPlayOffMatch("Round-of-16", "8", 14, 15);
                    break;
            }
        }

        public void PlayOffQuarterfinals(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForQuarterfinals[howManyRoundsPlayed])
            {
                case 1:
                    SetupAPlayOffMatch("Quarterfinals", "1", 0, 1);
                    break;
                case 2:
                    SetupAPlayOffMatch("Quarterfinals", "2", 2, 3);
                    break;
                case 3:
                    SetupAPlayOffMatch("Quarterfinals", "3", 4, 5);
                    break;
                case 4:
                    SetupAPlayOffMatch("Quarterfinals", "4", 6, 7);      
                    break;
            }
        }

        public void PlayOffSemifinals(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForSemifinals[howManyRoundsPlayed])
            {
                case 1:
                    SetupAPlayOffMatch("Semifinals", "1", 0, 1);
                    break;
                case 2:
                    SetupAPlayOffMatch("Semifinals", "2", 2, 3);
                    break;
            }
        }

        public void PlayOffFinal(List<int> list, int howManyRoundsPlayed)
        {
            switch (listOfEventsForFinal[howManyRoundsPlayed])
            {
                case 1:
                    SetupAPlayOffMatch("Final", "1", 0, 1);
                    break;
                case 2:
                    SetupAPlayOffMatch("Final", "2", 2, 3);
                    break;
            }
        }

        //This is a method to set up a match on the MATCHES tab.
        //To further randomize the matches, it is also randomized if a team is playing as a home or an away team.     
        Color homeColor = Color.DimGray;
        Color awayColor = Color.DimGray;
        CountryWithStats team1 = null;
        CountryWithStats team2 = null;
        string whatGroupIsSelected = null;
        public void SetupAMatch(string group, int selector1, int selector2)
        {            
            Random rng = new Random();
            int rngResult;
            switch (group)
            {
                case "A":
                    team1 = list_GroupA[selector1];
                    team2 = list_GroupA[selector2];
                    whatGroupIsSelected = "A";
                    break;
                case "B":
                    team1 = list_GroupB[selector1];
                    team2 = list_GroupB[selector2];
                    whatGroupIsSelected = "B";
                    break;
                case "C":
                    team1 = list_GroupC[selector1];
                    team2 = list_GroupC[selector2];
                    whatGroupIsSelected = "C";
                    break;
                case "D":
                    team1 = list_GroupD[selector1];
                    team2 = list_GroupD[selector2];
                    whatGroupIsSelected = "D";
                    break;
                case "E":
                    team1 = list_GroupE[selector1];
                    team2 = list_GroupE[selector2];
                    whatGroupIsSelected = "E";
                    break;
                case "F":
                    team1 = list_GroupF[selector1];
                    team2 = list_GroupF[selector2];
                    whatGroupIsSelected = "F";
                    break;
                case "G":
                    team1 = list_GroupG[selector1];
                    team2 = list_GroupG[selector2];
                    whatGroupIsSelected = "G";
                    break;
                case "H":
                    team1 = list_GroupH[selector1];
                    team2 = list_GroupH[selector2];
                    whatGroupIsSelected = "H";
                    break;
                case "Round-of-16":
                    team1 = playOffArray[selector1];
                    team2 = playOffArray[selector2];
                    break;
                case "Quarterfinals":
                    team1 = quarterfinalArray[selector1];
                    team2 = quarterfinalArray[selector2];
                    break;
                case "Semifinals":
                    team1 = semifinalArray[selector1];
                    team2 = semifinalArray[selector2];
                    break;
                case "Final":
                    team1 = finalArray[selector1];
                    team2 = finalArray[selector2];
                    break;
            }
            rngResult = rng.Next(2);
            if (rngResult == 0)
            {
                areTeamsSwapped = false;
                lblHomeTeam.Text = team1.Name;
                homeColor = TextToColor(team1.Colormain);
                lblAwayTeam.Text = team2.Name;
                if (team1.Colormain == team2.Colorsecondary)
                {
                    awayColor = TextToColor(team2.Colormain);
                }
                else
                {
                    awayColor = TextToColor(team2.Colorsecondary);
                }
                panel_Dress1_Paint(this, null);
                panel_Dress2_Paint(this, null);
            }
            else
            {
                areTeamsSwapped = true;
                lblHomeTeam.Text = team2.Name;
                homeColor = TextToColor(team2.Colormain);
                lblAwayTeam.Text = team1.Name;
                if (team2.Colormain == team1.Colorsecondary)
                {
                    awayColor = TextToColor(team1.Colormain);
                }
                else
                {
                    awayColor = TextToColor(team1.Colorsecondary);
                }
                panel_Dress1_Paint(this, null);
                panel_Dress2_Paint(this, null);
            }
        }

        public void SetupAPlayOffMatch(string round, string bracket, int selector1, int selector2)
        {
            whatGroupIsSelected = bracket;
            SetupAMatch(round, selector1, selector2);
        }

        private void textBoxTeam1_TextChanged(object sender, EventArgs e)
        {
            //Unlike in real football, in this app the highest amount of goals in a match is 9.
            //This is because during coding, i found the need to constantly have to reach for a mouse in order to focus on the other
            //textBox to be extremely annoying. Thanks to only single digit scores being allowed, the program is able to
            //automatically focus on the other textBox and then on the Submit button for the user.
            //If the user inserts anything that isn't a number, the program will delete it and automatically focus him on the textBox again.
            int intChecker;
            if (int.TryParse(textBoxTeam1.Text, out intChecker) && intChecker < 10 && intChecker > -1)
            {
                textBoxTeam2.Focus();
            }
            else
            {
                textBoxTeam1.Text = "";
                textBoxTeam1.Focus();
            }
        }
        private void textBoxTeam2_TextChanged(object sender, EventArgs e)
        {
            int intChecker;
            if (int.TryParse(textBoxTeam2.Text, out intChecker) && intChecker < 10 && intChecker > -1)
            {
                btnSubmitAndContinue.Focus();
            }
            else
            {
                textBoxTeam2.Text = "";
                textBoxTeam2.Focus();
            }
        }

        private void btnSubmitAndContinue_Click(object sender, EventArgs e)
        {               
            //Due to the fact that a team can play as either home or away teams, this method needs to be twice as long.
            //After each match result is submitted, the RoundX method is called with an incremented hMRP_Inserter (howManyRoundsPlayed).
            if (isItPlayOff == false && !string.IsNullOrEmpty(textBoxTeam1.Text) && !string.IsNullOrEmpty(textBoxTeam2.Text))
            {
                if (hMRP_Inserter < 15)
                {
                    hMRP_Inserter++;
                    ProcessMatchResult(whatGroupIsSelected, team1, team2);
                    textBoxTeam1.Text = "";
                    textBoxTeam2.Text = "";
                    textBoxTeam1.Focus();
                    switch (whatRoundIsIt)
                    {
                        case 1:
                            RoundOne(listOfEventsForARound, hMRP_Inserter);
                            break;
                        case 2:
                            RoundTwo(listOfEventsForARound, hMRP_Inserter);
                            break;
                        case 3:
                            RoundThree(listOfEventsForARound, hMRP_Inserter);
                            break;
                    }
                }
                else
                {
                    hMRP_Inserter++;
                    ProcessMatchResult(whatGroupIsSelected, team1, team2);
                    textBoxTeam1.Text = "";
                    textBoxTeam2.Text = "";
                    whatRoundIsIt++;
                    SortGroups();
                    if (whatRoundIsIt == 2)
                    {
                        btnStartRounds.Text = "Begin Round 2";
                        btnStartRounds.Enabled = true;
                        tabControl1.SelectedIndex = 2;
                        //After most tab changes, the code focuses on a button to allow for as little mouse use as possible,
                        //as having to constantly move between the mouse and the keyboard was an issue while testing this app.
                        //There are some exceptions:
                        //  During ties - To not force a certain choice on the user
                        //  After ties - to let the user check the final group stage results
                        //  After final - to prevent the user from accidentally quitting/resetting the app
                        //  On the Participants screen - Mouse use required
                        //  On startup
                        btnStartRounds.Focus();
                    }
                    else if (whatRoundIsIt == 3)
                    {                        
                        btnStartRounds.Text = "Begin Round 3";
                        btnStartRounds.Enabled = true;
                        tabControl1.SelectedIndex = 2;
                        btnStartRounds.Focus();
                    }
                    else if (whatRoundIsIt == 4)
                    {
                        btnStartRounds.Text = "Begin the Play-Offs";
                        btnStartPlayOffRounds.Text = "Begin the Round of 16";
                        btnStartRounds.Focus();
                    }
                    lblHomeTeam.Text = "";
                    lblAwayTeam.Text = "";
                    homeColor = Color.DimGray;
                    awayColor = Color.DimGray;
                    panel_Dress1_Paint(this, null);
                    panel_Dress2_Paint(this, null);
                    textBoxTeam1.Enabled = false;
                    textBoxTeam2.Enabled = false;
                    btnSubmitAndContinue.Enabled = false;                    
                }
            }
            else if (isItPlayOff == true && !string.IsNullOrEmpty(textBoxTeam1.Text) && !string.IsNullOrEmpty(textBoxTeam2.Text))
            {                
                if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                {
                    MessageBox.Show("Play Off matches cannot end with a tie! Please, resubmit with one of the teams winning.");
                    textBoxTeam1.Clear();
                    textBoxTeam2.Clear();
                    textBoxTeam1.Focus();
                }
                else if (((Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text)) && areTeamsSwapped == false) || ((Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text)) && areTeamsSwapped == true))
                {                    
                    switch (whatPlayOffRoundIsIt)
                    {
                        case "Round-of-16":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    quarterfinalArray[0] = team1;
                                    break;
                                case "2":
                                    quarterfinalArray[1] = team1;
                                    break;
                                case "3":
                                    quarterfinalArray[2] = team1;
                                    break;
                                case "4":
                                    quarterfinalArray[3] = team1;
                                    break;
                                case "5":
                                    quarterfinalArray[4] = team1;
                                    break;
                                case "6":
                                    quarterfinalArray[5] = team1;
                                    break;
                                case "7":
                                    quarterfinalArray[6] = team1;
                                    break;
                                case "8":
                                    quarterfinalArray[7] = team1;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 7)
                            {
                                btnStartPlayOffRounds.Text = "Begin the Quarterfinals";
                                playOff_Inserter++;
                                PlayOffRoundOf16(listOfEventsForRoundOf16, playOff_Inserter);
                            }
                            else
                            {
                                SetQuarterfinals();
                            }
                            break;
                        case "Quarterfinals":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    semifinalArray[0] = team1;
                                    break;
                                case "2":
                                    semifinalArray[1] = team1;
                                    break;
                                case "3":
                                    semifinalArray[2] = team1;
                                    break;
                                case "4":
                                    semifinalArray[3] = team1;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 3)
                            {
                                btnStartPlayOffRounds.Text = "Begin the Semifinals";
                                playOff_Inserter++;
                                PlayOffQuarterfinals(listOfEventsForQuarterfinals, playOff_Inserter);
                            }
                            else
                            {
                                SetSemifinals();
                            }
                            break;
                        case "Semifinals":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    finalArray[0] = team1;
                                    finalArray[2] = team2;
                                    break;
                                case "2":
                                    finalArray[1] = team1;
                                    finalArray[3] = team2;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 1)
                            {                                
                                playOff_Inserter++;
                                PlayOffSemifinals(listOfEventsForSemifinals, playOff_Inserter);
                            }
                            else
                            {                                
                                SetFinals();
                            }
                            break;
                        case "Final":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    resultsArray[0] = team1;
                                    resultsArray[1] = team2;
                                    break;
                                case "2":
                                    resultsArray[2] = team1;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 1)
                            {
                                playOff_Inserter++;
                                PlayOffFinal(listOfEventsForFinal, playOff_Inserter);
                            }
                            else
                            {
                                SetResults();
                            }
                            break;
                    }                    
                }
                else
                {
                    switch (whatPlayOffRoundIsIt)
                    {
                        case "Round-of-16":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    quarterfinalArray[0] = team2;
                                    break;
                                case "2":
                                    quarterfinalArray[1] = team2;
                                    break;
                                case "3":
                                    quarterfinalArray[2] = team2;
                                    break;
                                case "4":
                                    quarterfinalArray[3] = team2;
                                    break;
                                case "5":
                                    quarterfinalArray[4] = team2;
                                    break;
                                case "6":
                                    quarterfinalArray[5] = team2;
                                    break;
                                case "7":
                                    quarterfinalArray[6] = team2;
                                    break;
                                case "8":
                                    quarterfinalArray[7] = team2;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 7)
                            {
                                playOff_Inserter++;
                                PlayOffRoundOf16(listOfEventsForRoundOf16, playOff_Inserter);
                            }
                            else
                            {
                                SetQuarterfinals();
                            }
                            break;
                        case "Quarterfinals":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    semifinalArray[0] = team2;
                                    break;
                                case "2":
                                    semifinalArray[1] = team2;
                                    break;
                                case "3":
                                    semifinalArray[2] = team2;
                                    break;
                                case "4":
                                    semifinalArray[3] = team2;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 3)
                            {
                                playOff_Inserter++;
                                PlayOffQuarterfinals(listOfEventsForQuarterfinals, playOff_Inserter);
                            }
                            else
                            {
                                SetSemifinals();
                            }
                            break;
                        case "Semifinals":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    finalArray[0] = team2;
                                    finalArray[2] = team1;
                                    break;
                                case "2":
                                    finalArray[1] = team2;
                                    finalArray[3] = team1;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 1)
                            {
                                playOff_Inserter++;
                                PlayOffSemifinals(listOfEventsForSemifinals, playOff_Inserter);
                            }
                            else
                            {
                                SetFinals();
                            }
                            break;
                        case "Final":
                            switch (whatGroupIsSelected)
                            {
                                case "1":
                                    resultsArray[0] = team2;
                                    resultsArray[1] = team1;
                                    break;
                                case "2":
                                    resultsArray[2] = team2;
                                    break;
                            }
                            textBoxTeam1.Text = "";
                            textBoxTeam2.Text = "";
                            textBoxTeam1.Focus();
                            if (playOff_Inserter < 1)
                            {
                                playOff_Inserter++;
                                PlayOffFinal(listOfEventsForFinal, playOff_Inserter);
                            }
                            else
                            {
                                SetResults();
                            }
                            break;
                    }                    
                }                
            }
            else
            {
                MessageBox.Show("You pressed the button without giving both teams a score!");
            }
        }

        int score_sg1, score_sg2, score_sg3, score_sg4;
        int stepInSortGroups = 0;
        public void SortGroups()
        {
            //SortGroups exists to update the dataGridViews and also as a "hub method" of sorts for ties.
            if (whatRoundIsIt == 4)
            {
                switch (stepInSortGroups)
                {
                    case 0:
                        SolveTies(list_GroupA[0], list_GroupA[1], list_GroupA[2], list_GroupA[3], "A");
                        break;
                    case 1:
                        SetScore(list_GroupA[0], list_GroupA[1], list_GroupA[2], list_GroupA[3], "A");
                        break;
                    case 2:
                        SolveTies(list_GroupB[0], list_GroupB[1], list_GroupB[2], list_GroupB[3], "B");
                        break;
                    case 3:
                        SetScore(list_GroupB[0], list_GroupB[1], list_GroupB[2], list_GroupB[3], "B");
                        break;
                    case 4:
                        SolveTies(list_GroupC[0], list_GroupC[1], list_GroupC[2], list_GroupC[3], "C");
                        break;
                    case 5:
                        SetScore(list_GroupC[0], list_GroupC[1], list_GroupC[2], list_GroupC[3], "C");
                        break;
                    case 6:
                        SolveTies(list_GroupD[0], list_GroupD[1], list_GroupD[2], list_GroupD[3], "D");
                        break;
                    case 7:
                        SetScore(list_GroupD[0], list_GroupD[1], list_GroupD[2], list_GroupD[3], "D");
                        break;
                    case 8:
                        SolveTies(list_GroupE[0], list_GroupE[1], list_GroupE[2], list_GroupE[3], "E");
                        break;
                    case 9:
                        SetScore(list_GroupE[0], list_GroupE[1], list_GroupE[2], list_GroupE[3], "E");
                        break;
                    case 10:
                        SolveTies(list_GroupF[0], list_GroupF[1], list_GroupF[2], list_GroupF[3], "F");
                        break;
                    case 11:
                        SetScore(list_GroupF[0], list_GroupF[1], list_GroupF[2], list_GroupF[3], "F");
                        break;
                    case 12:
                        SolveTies(list_GroupG[0], list_GroupG[1], list_GroupG[2], list_GroupG[3], "G");
                        break;
                    case 13:
                        SetScore(list_GroupG[0], list_GroupG[1], list_GroupG[2], list_GroupG[3], "G");
                        break;
                    case 14:
                        SolveTies(list_GroupH[0], list_GroupH[1], list_GroupH[2], list_GroupH[3], "H");                        
                        break;
                    case 15:
                        SetScore(list_GroupH[0], list_GroupH[1], list_GroupH[2], list_GroupH[3], "H");
                        lbl_TypeOfTie.Text = "";
                        dgw_ties.Rows.Clear();
                        btn_tie_1.Enabled = false; btn_tie_2.Enabled = false; btn_tie_3.Enabled = false; btn_tie_4.Enabled = false;
                        btn_tie_1.Text = "These";
                        btn_tie_2.Text = "Are";
                        btn_tie_3.Text = "Indeed";
                        btn_tie_4.Text = "Buttons";
                        btnStartRounds.Enabled = true;
                        tabControl1.SelectedIndex = 2;
                        btnStartRounds.Focus();
                        break;
                }
            }
            else
            {
                SetScore(list_GroupA[0], list_GroupA[1], list_GroupA[2], list_GroupA[3], "A");
                SetScore(list_GroupB[0], list_GroupB[1], list_GroupB[2], list_GroupB[3], "B");
                SetScore(list_GroupC[0], list_GroupC[1], list_GroupC[2], list_GroupC[3], "C");
                SetScore(list_GroupD[0], list_GroupD[1], list_GroupD[2], list_GroupD[3], "D");
                SetScore(list_GroupE[0], list_GroupE[1], list_GroupE[2], list_GroupE[3], "E");
                SetScore(list_GroupF[0], list_GroupF[1], list_GroupF[2], list_GroupF[3], "F");
                SetScore(list_GroupG[0], list_GroupG[1], list_GroupG[2], list_GroupG[3], "G");
                SetScore(list_GroupH[0], list_GroupH[1], list_GroupH[2], list_GroupH[3], "H");
            }                      
        }

        //Each one of these Set[PlayOff round] serves to insert team names into PLAY-OFF listBoxes and for other
        //actions such as renaming and (Dis)enabling buttons after a Play-Off round.
        public void SetQuarterfinals()
        {
            PrepareButtonsForNextPlayOffRound();
            quart1.Items.Add(quarterfinalArray[0].Name);
            quart1.Items.Add(quarterfinalArray[1].Name);
            quart2.Items.Add(quarterfinalArray[2].Name);
            quart2.Items.Add(quarterfinalArray[3].Name);
            quart3.Items.Add(quarterfinalArray[4].Name);
            quart3.Items.Add(quarterfinalArray[5].Name);
            quart4.Items.Add(quarterfinalArray[6].Name);
            quart4.Items.Add(quarterfinalArray[7].Name);
            whatPlayOffRoundIsIt = "Quarterfinals";
            btnStartPlayOffRounds.Enabled = true;
            tabControl1.SelectedIndex = 5;
            btnStartPlayOffRounds.Focus();
        }

        public void SetSemifinals()
        {
            PrepareButtonsForNextPlayOffRound();
            semi1.Items.Add(semifinalArray[0].Name);
            semi1.Items.Add(semifinalArray[1].Name);
            semi2.Items.Add(semifinalArray[2].Name);
            semi2.Items.Add(semifinalArray[3].Name);
            whatPlayOffRoundIsIt = "Semifinals";
            btnStartPlayOffRounds.Enabled = true;
            tabControl1.SelectedIndex = 5;
            btnStartPlayOffRounds.Focus();
        }

        public void SetFinals()
        {
            PrepareButtonsForNextPlayOffRound();
            finaleListBox.Items.Add(finalArray[0].Name);
            finaleListBox.Items.Add(finalArray[1].Name);
            btnStartPlayOffRounds.Text = "Begin the Final";
            thirdPlaceMatch.Items.Add(finalArray[2].Name);
            thirdPlaceMatch.Items.Add(finalArray[3].Name);
            whatPlayOffRoundIsIt = "Final";
            btnStartPlayOffRounds.Enabled = true;
            tabControl1.SelectedIndex = 5;
            btnStartPlayOffRounds.Focus();
        }

        //SetResults is an exception because it sets up the RESULTS tab.
        Color winnerColor = Color.DimGray;
        public void SetResults()
        {
            PrepareButtonsForNextPlayOffRound();
            winnerColor = TextToColor(resultsArray[0].Colormain);
            panel_Winner_Paint(this, null);
            lblWinner.Text = resultsArray[0].Name;
            labelSecondPlace.Text = "Second place: " + resultsArray[1].Name;
            labelThirdPlace.Text = "Third place: " + resultsArray[2].Name;
            whatPlayOffRoundIsIt = "Results";
            btnStartPlayOffRounds.Text = "";
            tabControl1.SelectedIndex = 6;
            panel_Winner.Focus();
        }

        public void PrepareButtonsForNextPlayOffRound()
        {
            lblHomeTeam.Text = "";
            lblAwayTeam.Text = "";
            homeColor = Color.DimGray;
            awayColor = Color.DimGray;
            panel_Dress1_Paint(this, null);
            panel_Dress2_Paint(this, null);
            textBoxTeam1.Enabled = false;
            textBoxTeam2.Enabled = false;
            btnSubmitAndContinue.Enabled = false;            
        }

        int whatButtonGotPressed = 99;
        int secondButtonPressed = 99;
        int whatTieAreWeSolving = 0;
        public void SolveTies(CountryWithStats sg1, CountryWithStats sg2, CountryWithStats sg3, CountryWithStats sg4, string group)
        {
            //Method for ties. In the real world cup ties are always broken up, however due to this app only having an input for
            //score, ties are bound to occur.
            //The biggest challenge was aligning the position of the teams in the group List where they aren't sorted alphabetically
            //with the dataGridView where they are.
            //I solved this issue by using orderOfButtons[] as an intermediary array.
            tieScore1 = 0;
            tieScore2 = 0;
            tieScore3 = 0;
            tieScore4 = 0;

            score_sg1 = sg1.Points * 1000 + sg1.Goals * 10;
            score_sg2 = sg2.Points * 1000 + sg2.Goals * 10;
            score_sg3 = sg3.Points * 1000 + sg3.Goals * 10;
            score_sg4 = sg4.Points * 1000 + sg4.Goals * 10;

            Dictionary<string, int> scores = new Dictionary<string, int>();
            scores.Add("A", score_sg1);
            scores.Add("B", score_sg2);
            scores.Add("C", score_sg3);
            scores.Add("D", score_sg4);

            var sortedDict = scores
                .OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            scores = sortedDict;            

            dgw_ties.Rows.Clear();
            int buttonCounter = 0;
            string buttonNameInserter = "";
            foreach (KeyValuePair<string, int> s in scores)
            {
                switch (s.Key)
                {
                    case "A":
                        dgw_ties.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                        buttonNameInserter = sg1.Name;
                        orderOfButtons[buttonCounter] = "A";
                        break;
                    case "B":
                        dgw_ties.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                        buttonNameInserter = sg2.Name;
                        orderOfButtons[buttonCounter] = "B";
                        break;
                    case "C":
                        dgw_ties.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                        buttonNameInserter = sg3.Name;
                        orderOfButtons[buttonCounter] = "C";
                        break;
                    case "D":
                        dgw_ties.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                        buttonNameInserter = sg4.Name;
                        orderOfButtons[buttonCounter] = "D";
                        break;
                }                

                if (whatRoundIsIt == 4)
                {
                    switch (buttonCounter)
                    {
                        case 0:
                            btn_tie_1.Text = buttonNameInserter;
                            break;
                        case 1:
                            btn_tie_2.Text = buttonNameInserter;
                            break;
                        case 2:
                            btn_tie_3.Text = buttonNameInserter;
                            break;
                        case 3:
                            btn_tie_4.Text = buttonNameInserter;
                            break;
                    }
                    buttonCounter++;
                }
            }

            int counter = 0;
            int score1 = 0;
            int score2 = 0;
            int score3 = 0;
            int score4 = 0;
            foreach (KeyValuePair<string, int> s in scores)
            {
                switch (counter)
                {
                    case 0:
                        score1 = s.Value;
                        counter++;
                        break;
                    case 1:
                        score2 = s.Value;
                        counter++;
                        break;
                    case 2:
                        score3 = s.Value;
                        counter++;
                        break;
                    case 3:
                        score4 = s.Value;
                        break;
                }
            }

            //This "if" condition is to choose what type of a tie (if any) has occured.
            if (whatRoundIsIt == 4)
            {
                if (score1 == score2 && score2 == score3 && score3 == score4)
                {
                    whatTieAreWeSolving = 1;
                    buttonsClickedAlready = 0;
                    btn_tie_1.Enabled = true;
                    btn_tie_2.Enabled = true;
                    btn_tie_3.Enabled = true;
                    btn_tie_4.Enabled = true;
                    tabControl1.SelectedIndex = 4;
                    lbl_TypeOfTie.Text = "There is a four-way tie in Group " + group + "!";
                    lbl_WhatToClick.Text = "Click on the team that should get first place in this group:";
                }
                else if (score1 == score2 && score2 == score3 && score3 != score4)
                {
                    whatTieAreWeSolving = 2;
                    buttonsClickedAlready = 0;
                    btn_tie_1.Enabled = true;
                    btn_tie_2.Enabled = true;
                    btn_tie_3.Enabled = true;
                    btn_tie_4.Enabled = false;
                    tabControl1.SelectedIndex = 4;
                    lbl_TypeOfTie.Text = "There is a three-way tie in Group " + group + "!";
                    lbl_WhatToClick.Text = "Click on the team that should get first place in this group:";
                }
                else if (score1 != score2 && score2 == score3 && score3 == score4)
                {
                    whatTieAreWeSolving = 3;
                    buttonsClickedAlready = 0;
                    btn_tie_1.Enabled = false;
                    btn_tie_2.Enabled = true;
                    btn_tie_3.Enabled = true;
                    btn_tie_4.Enabled = true;
                    tabControl1.SelectedIndex = 4;
                    lbl_TypeOfTie.Text = "There is a three-way tie in Group " + group + "!";
                    lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                }
                else if (score1 != score2 && score2 == score3 && score3 != score4)
                {
                    whatTieAreWeSolving = 4;
                    buttonsClickedAlready = 0;
                    btn_tie_1.Enabled = false;
                    btn_tie_2.Enabled = true;
                    btn_tie_3.Enabled = true;
                    btn_tie_4.Enabled = false;
                    tabControl1.SelectedIndex = 4;
                    lbl_TypeOfTie.Text = "There is a two-way tie in Group " + group + "!";
                    lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                }
                else if (score1 == score2 && score2 != score3)
                {
                    whatTieAreWeSolving = 5;
                    buttonsClickedAlready = 0;
                    btn_tie_1.Enabled = true;
                    btn_tie_2.Enabled = true;
                    btn_tie_3.Enabled = false;
                    btn_tie_4.Enabled = false;
                    tabControl1.SelectedIndex = 4;
                    lbl_TypeOfTie.Text = "There is a two-way tie in Group " + group + "!";
                    lbl_WhatToClick.Text = "Click on the team that should go into the Play-Offs from the first place:";
                }
                else
                {
                    stepInSortGroups++;
                    SortGroups();
                }                
            }
        }

        int tieScore1 = 0;
        int tieScore2 = 0;
        int tieScore3 = 0;
        int tieScore4 = 0;
        //This method calculates the "actual" score of each team after a round.
        //The "actual" hidden score using which the teams are sorted is calculated using this formula:
        //Hidden Score = Points * 1000 + Goals * 10 + tieScore
        //The most possible goals teams can score in the group stage is 36 => 360 is smaller than 1000 => Points have a priority
        //tieScore is given during tiebreaking on the TIES screen. It's only individual points, either 2 or 1.
        public void SetScore(CountryWithStats sg1, CountryWithStats sg2, CountryWithStats sg3, CountryWithStats sg4, string group)
        {
            score_sg1 = sg1.Points * 1000 + sg1.Goals * 10 + tieScore1;
            score_sg2 = sg2.Points * 1000 + sg2.Goals * 10 + tieScore2;
            score_sg3 = sg3.Points * 1000 + sg3.Goals * 10 + tieScore3;
            score_sg4 = sg4.Points * 1000 + sg4.Goals * 10 + tieScore4;
            
            Dictionary<string, int> scores = new Dictionary<string, int>();
            scores.Add("A", score_sg1);
            scores.Add("B", score_sg2);
            scores.Add("C", score_sg3);
            scores.Add("D", score_sg4);

            var sortedDict = scores
                .OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            scores = sortedDict;

            int playOffInsertCounter = 0;
            CountryWithStats playOffInserter = null;
            switch (group)
            {
                //This method is sadly very long due to the "8 programs in 1" issue - as mentioned on line 802.
                case "A":
                    dgw_A.Rows.Clear();                                       
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_A.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_A.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_A.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_A.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[0] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[9] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }                    
                    break;
                case "B":
                    dgw_B.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_B.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_B.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_B.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_B.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[8] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[1] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "C":
                    dgw_C.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_C.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_C.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_C.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_C.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[2] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[11] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "D":
                    dgw_D.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_D.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_D.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_D.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_D.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[10] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[3] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "E":
                    dgw_E.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_E.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_E.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_E.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_E.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[4] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[13] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "F":
                    dgw_F.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_F.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_F.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_F.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_F.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[12] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[5] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "G":
                    dgw_G.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_G.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_G.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_G.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_G.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[6] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[15] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
                case "H":
                    dgw_H.Rows.Clear();
                    foreach (KeyValuePair<string, int> s in scores)
                    {
                        switch (s.Key)
                        {
                            case "A":
                                dgw_H.Rows.Add(sg1.Name, sg1.Wins, sg1.Draws, sg1.Losses, sg1.Goals, sg1.Points);
                                playOffInserter = sg1;
                                break;
                            case "B":
                                dgw_H.Rows.Add(sg2.Name, sg2.Wins, sg2.Draws, sg2.Losses, sg2.Goals, sg2.Points);
                                playOffInserter = sg2;
                                break;
                            case "C":
                                dgw_H.Rows.Add(sg3.Name, sg3.Wins, sg3.Draws, sg3.Losses, sg3.Goals, sg3.Points);
                                playOffInserter = sg3;
                                break;
                            case "D":
                                dgw_H.Rows.Add(sg4.Name, sg4.Wins, sg4.Draws, sg4.Losses, sg4.Goals, sg4.Points);
                                playOffInserter = sg4;
                                break;
                        }
                        if (whatRoundIsIt == 4 && playOffInsertCounter == 0)
                        {
                            playOffArray[14] = playOffInserter;
                        }
                        else if (whatRoundIsIt == 4 && playOffInsertCounter == 1)
                        {
                            playOffArray[7] = playOffInserter;
                        }
                        playOffInsertCounter++;
                    }
                    break;
            }            

            if (whatRoundIsIt == 4)
            {
                stepInSortGroups++;
                SortGroups();
            }            
        }

        bool areTeamsSwapped = false;
        //This method is used for changing the values (Points, Goals, Wins, Draws, Losses) of each team depending on the match results.
        //The method is sadly very long due to the "8 programs in 1" issue - as mentioned on line 802 and due to the need
        //to account for each team being able to be either home or away.
        public void ProcessMatchResult(string whatGroupIsSelectedInput, CountryWithStats t1, CountryWithStats t2)
        {
            int index1, index2;            
            switch (whatGroupIsSelectedInput)
            {
                case "A":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupA.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupA.FindIndex(x => x.Name == t2.Name);
                        list_GroupA[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupA[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupA.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupA.FindIndex(x => x.Name == t1.Name);
                        list_GroupA[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupA[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupA[index1].Wins = list_GroupA[index1].Wins + 1;
                        list_GroupA[index2].Losses = list_GroupA[index2].Losses + 1;
                        list_GroupA[index1].Points = list_GroupA[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupA[index2].Wins = list_GroupA[index2].Wins + 1;
                        list_GroupA[index1].Losses = list_GroupA[index1].Losses + 1;
                        list_GroupA[index2].Points = list_GroupA[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupA[index1].Draws = list_GroupA[index1].Draws + 1;
                        list_GroupA[index2].Draws = list_GroupA[index2].Draws + 1;
                        list_GroupA[index1].Points = list_GroupA[index1].Points + 1;
                        list_GroupA[index2].Points = list_GroupA[index2].Points + 1;
                    }                    
                    break;
                case "B":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupB.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupB.FindIndex(x => x.Name == t2.Name);
                        list_GroupB[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupB[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupB.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupB.FindIndex(x => x.Name == t1.Name);
                        list_GroupB[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupB[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupB[index1].Wins = list_GroupB[index1].Wins + 1;
                        list_GroupB[index2].Losses = list_GroupB[index2].Losses + 1;
                        list_GroupB[index1].Points = list_GroupB[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupB[index2].Wins = list_GroupB[index2].Wins + 1;
                        list_GroupB[index1].Losses = list_GroupB[index1].Losses + 1;
                        list_GroupB[index2].Points = list_GroupB[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupB[index1].Draws = list_GroupB[index1].Draws + 1;
                        list_GroupB[index2].Draws = list_GroupB[index2].Draws + 1;
                        list_GroupB[index1].Points = list_GroupB[index1].Points + 1;
                        list_GroupB[index2].Points = list_GroupB[index2].Points + 1;
                    }
                    break;
                case "C":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupC.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupC.FindIndex(x => x.Name == t2.Name);
                        list_GroupC[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupC[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupC.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupC.FindIndex(x => x.Name == t1.Name);
                        list_GroupC[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupC[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupC[index1].Wins = list_GroupC[index1].Wins + 1;
                        list_GroupC[index2].Losses = list_GroupC[index2].Losses + 1;
                        list_GroupC[index1].Points = list_GroupC[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupC[index2].Wins = list_GroupC[index2].Wins + 1;
                        list_GroupC[index1].Losses = list_GroupC[index1].Losses + 1;
                        list_GroupC[index2].Points = list_GroupC[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupC[index1].Draws = list_GroupC[index1].Draws + 1;
                        list_GroupC[index2].Draws = list_GroupC[index2].Draws + 1;
                        list_GroupC[index1].Points = list_GroupC[index1].Points + 1;
                        list_GroupC[index2].Points = list_GroupC[index2].Points + 1;
                    }
                    break;
                case "D":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupD.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupD.FindIndex(x => x.Name == t2.Name);
                        list_GroupD[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupD[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupD.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupD.FindIndex(x => x.Name == t1.Name);
                        list_GroupD[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupD[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupD[index1].Wins = list_GroupD[index1].Wins + 1;
                        list_GroupD[index2].Losses = list_GroupD[index2].Losses + 1;
                        list_GroupD[index1].Points = list_GroupD[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupD[index2].Wins = list_GroupD[index2].Wins + 1;
                        list_GroupD[index1].Losses = list_GroupD[index1].Losses + 1;
                        list_GroupD[index2].Points = list_GroupD[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupD[index1].Draws = list_GroupD[index1].Draws + 1;
                        list_GroupD[index2].Draws = list_GroupD[index2].Draws + 1;
                        list_GroupD[index1].Points = list_GroupD[index1].Points + 1;
                        list_GroupD[index2].Points = list_GroupD[index2].Points + 1;
                    }
                    break;
                case "E":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupE.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupE.FindIndex(x => x.Name == t2.Name);
                        list_GroupE[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupE[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupE.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupE.FindIndex(x => x.Name == t1.Name);
                        list_GroupE[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupE[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupE[index1].Wins = list_GroupE[index1].Wins + 1;
                        list_GroupE[index2].Losses = list_GroupE[index2].Losses + 1;
                        list_GroupE[index1].Points = list_GroupE[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupE[index2].Wins = list_GroupE[index2].Wins + 1;
                        list_GroupE[index1].Losses = list_GroupE[index1].Losses + 1;
                        list_GroupE[index2].Points = list_GroupE[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupE[index1].Draws = list_GroupE[index1].Draws + 1;
                        list_GroupE[index2].Draws = list_GroupE[index2].Draws + 1;
                        list_GroupE[index1].Points = list_GroupE[index1].Points + 1;
                        list_GroupE[index2].Points = list_GroupE[index2].Points + 1;
                    }
                    break;
                case "F":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupF.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupF.FindIndex(x => x.Name == t2.Name);
                        list_GroupF[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupF[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupF.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupF.FindIndex(x => x.Name == t1.Name);
                        list_GroupF[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupF[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupF[index1].Wins = list_GroupF[index1].Wins + 1;
                        list_GroupF[index2].Losses = list_GroupF[index2].Losses + 1;
                        list_GroupF[index1].Points = list_GroupF[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupF[index2].Wins = list_GroupF[index2].Wins + 1;
                        list_GroupF[index1].Losses = list_GroupF[index1].Losses + 1;
                        list_GroupF[index2].Points = list_GroupF[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupF[index1].Draws = list_GroupF[index1].Draws + 1;
                        list_GroupF[index2].Draws = list_GroupF[index2].Draws + 1;
                        list_GroupF[index1].Points = list_GroupF[index1].Points + 1;
                        list_GroupF[index2].Points = list_GroupF[index2].Points + 1;
                    }
                    break;
                case "G":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupG.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupG.FindIndex(x => x.Name == t2.Name);
                        list_GroupG[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupG[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupG.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupG.FindIndex(x => x.Name == t1.Name);
                        list_GroupG[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupG[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupG[index1].Wins = list_GroupG[index1].Wins + 1;
                        list_GroupG[index2].Losses = list_GroupG[index2].Losses + 1;
                        list_GroupG[index1].Points = list_GroupG[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupG[index2].Wins = list_GroupG[index2].Wins + 1;
                        list_GroupG[index1].Losses = list_GroupG[index1].Losses + 1;
                        list_GroupG[index2].Points = list_GroupG[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupG[index1].Draws = list_GroupG[index1].Draws + 1;
                        list_GroupG[index2].Draws = list_GroupG[index2].Draws + 1;
                        list_GroupG[index1].Points = list_GroupG[index1].Points + 1;
                        list_GroupG[index2].Points = list_GroupG[index2].Points + 1;
                    }
                    break;
                case "H":
                    if (areTeamsSwapped == false)
                    {
                        index1 = list_GroupH.FindIndex(x => x.Name == t1.Name);
                        index2 = list_GroupH.FindIndex(x => x.Name == t2.Name);
                        list_GroupH[index1].Goals = t1.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupH[index2].Goals = t2.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    else
                    {
                        index1 = list_GroupH.FindIndex(x => x.Name == t2.Name);
                        index2 = list_GroupH.FindIndex(x => x.Name == t1.Name);
                        list_GroupH[index1].Goals = t2.Goals + Convert.ToInt32(textBoxTeam1.Text) - Convert.ToInt32(textBoxTeam2.Text);
                        list_GroupH[index2].Goals = t1.Goals + Convert.ToInt32(textBoxTeam2.Text) - Convert.ToInt32(textBoxTeam1.Text);
                    }
                    if (Convert.ToInt32(textBoxTeam1.Text) > Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupH[index1].Wins = list_GroupH[index1].Wins + 1;
                        list_GroupH[index2].Losses = list_GroupH[index2].Losses + 1;
                        list_GroupH[index1].Points = list_GroupH[index1].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) < Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupH[index2].Wins = list_GroupH[index2].Wins + 1;
                        list_GroupH[index1].Losses = list_GroupH[index1].Losses + 1;
                        list_GroupH[index2].Points = list_GroupH[index2].Points + 3;
                    }
                    else if (Convert.ToInt32(textBoxTeam1.Text) == Convert.ToInt32(textBoxTeam2.Text))
                    {
                        list_GroupH[index1].Draws = list_GroupH[index1].Draws + 1;
                        list_GroupH[index2].Draws = list_GroupH[index2].Draws + 1;
                        list_GroupH[index1].Points = list_GroupH[index1].Points + 1;
                        list_GroupH[index2].Points = list_GroupH[index2].Points + 1;
                    }
                    break;
            }
        }

        //Methods for tiebreaking buttons.
        //Each button only has code for the ties that affect its position
        int buttonsClickedAlready = 0;
        private void btn_tie_1_Click(object sender, EventArgs e)
        {
            btn_tie_1.Enabled = false;
            lbl_TypeOfTie.Focus();
            
            switch (whatTieAreWeSolving)
            {
                case 1:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 0;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 0;
                    }
                    break;
                case 2:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 0;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 0;
                    }
                    break;
                case 5:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 0;
                    break;
            }
            CheckIfEnoughButtonsGotPressed();
        }

        private void btn_tie_2_Click(object sender, EventArgs e)
        {
            btn_tie_2.Enabled = false;
            lbl_TypeOfTie.Focus();

            switch (whatTieAreWeSolving)
            {
                case 1:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 1;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 1;
                    }
                    break;
                case 2:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 1;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 1;
                    }
                    break;
                case 3:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 1;
                    break;
                case 4:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 1;
                    break;
                case 5:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 1;
                    break;
            }
            CheckIfEnoughButtonsGotPressed();
        }

        private void btn_tie_3_Click(object sender, EventArgs e)
        {
            btn_tie_3.Enabled = false;
            lbl_TypeOfTie.Focus();

            switch (whatTieAreWeSolving)
            {
                case 1:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 2;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 2;
                    }
                    break;
                case 2:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 2;
                        lbl_WhatToClick.Text = "Click on the team that should get second place in this group:";
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 2;
                    }
                    break;
                case 3:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 2;
                    break;
                case 4:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 2;
                    break;
            }
            CheckIfEnoughButtonsGotPressed();
        }

        private void btn_tie_4_Click(object sender, EventArgs e)
        {
            btn_tie_4.Enabled = false;
            lbl_TypeOfTie.Focus();

            switch (whatTieAreWeSolving)
            {
                case 1:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 3;
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 3;
                    }
                    break;
                case 2:
                    if (buttonsClickedAlready == 0)
                    {
                        buttonsClickedAlready++;
                        whatButtonGotPressed = 3;
                    }
                    else
                    {
                        buttonsClickedAlready++;
                        secondButtonPressed = 3;
                    }
                    break;
                case 3:
                    buttonsClickedAlready++;
                    whatButtonGotPressed = 3;
                    break;
            }
            CheckIfEnoughButtonsGotPressed();
        }
        
        //Some ties require two buttons to be pressed - This method solves that issue.
        private void CheckIfEnoughButtonsGotPressed()
        {
            switch (whatTieAreWeSolving)
            {
                case 1:
                    if (buttonsClickedAlready == 2)
                    {
                        stepInSortGroups++;
                        CastButtonsIntoPoints(orderOfButtons, whatButtonGotPressed, secondButtonPressed);
                        SortGroups();
                    }
                    break;
                case 2:
                    if (buttonsClickedAlready == 2)
                    {
                        stepInSortGroups++;
                        CastButtonsIntoPoints(orderOfButtons, whatButtonGotPressed, secondButtonPressed);
                        SortGroups();
                    }
                    break;
                case 3:
                    if (buttonsClickedAlready == 1)
                    {
                        stepInSortGroups++;
                        CastButtonsIntoPoints(orderOfButtons, whatButtonGotPressed, secondButtonPressed);
                        SortGroups();
                    }
                    break;
                case 4:
                    if (buttonsClickedAlready == 1)
                    {
                        stepInSortGroups++;
                        CastButtonsIntoPoints(orderOfButtons, whatButtonGotPressed, secondButtonPressed);
                        SortGroups();
                    }
                    break;
                case 5:
                    if (buttonsClickedAlready == 1)
                    {
                        stepInSortGroups++;
                        CastButtonsIntoPoints(orderOfButtons, whatButtonGotPressed, secondButtonPressed);
                        SortGroups();
                    }
                    break;
            }
        }

        public void CastButtonsIntoPoints(string[] array, int whatButtonGotPressed, int secondButtonPressed)
        {
            string x = "X";
            string y = "Y";
            if (whatButtonGotPressed != 99) //99 is a stand-in value for "no button was pressed yet"
            {
                x = array[whatButtonGotPressed];
            }
            if (secondButtonPressed != 99)
            {
                y = array[secondButtonPressed];
            }
            switch (x)
            {
                case "A":
                    tieScore1 = 2;
                    break;
                case "B":
                    tieScore2 = 2;
                    break;
                case "C":
                    tieScore3 = 2;
                    break;
                case "D":
                    tieScore4 = 2;
                    break;
            }

            switch (y)
            {
                case "A":
                    tieScore1 = 1;
                    break;
                case "B":
                    tieScore2 = 1;
                    break;
                case "C":
                    tieScore3 = 1;
                    break;
                case "D":
                    tieScore4 = 1;
                    break;
            }
        }

        int playOff_Inserter;
        List<int> listOfEventsForRoundOf16;
        List<int> listOfEventsForQuarterfinals;
        List<int> listOfEventsForSemifinals;
        List<int> listOfEventsForFinal;
        string whatPlayOffRoundIsIt = "Round-of-16";
        //Same as btnStartRounds_Click on line 943 but each Play-Off round has their own List due to the varying match amount.
        private void btnStartPlayOffRounds_Click(object sender, EventArgs e)
        {
            textBoxTeam1.Enabled = true;
            textBoxTeam2.Enabled = true;
            btnSubmitAndContinue.Enabled = true;
            btnStartPlayOffRounds.Enabled = false;
            switch (whatPlayOffRoundIsIt)
            {
                case "Round-of-16":
                    playOff_Inserter = 0;
                    tabControl1.SelectedIndex = 3;
                    listOfEventsForRoundOf16 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
                    ListShuffler.Shuffle(listOfEventsForRoundOf16);
                    PlayOffRoundOf16(listOfEventsForRoundOf16, playOff_Inserter);
                    break;
                case "Quarterfinals":
                    playOff_Inserter = 0;
                    tabControl1.SelectedIndex = 3;
                    listOfEventsForQuarterfinals = new List<int> { 1, 2, 3, 4 };
                    ListShuffler.Shuffle(listOfEventsForQuarterfinals);
                    PlayOffQuarterfinals(listOfEventsForQuarterfinals, playOff_Inserter);
                    break;
                case "Semifinals":
                    playOff_Inserter = 0;
                    tabControl1.SelectedIndex = 3;
                    listOfEventsForSemifinals = new List<int> { 1, 2 };
                    ListShuffler.Shuffle(listOfEventsForSemifinals);
                    PlayOffSemifinals(listOfEventsForSemifinals, playOff_Inserter);
                    break;
                case "Final":
                    playOff_Inserter = 0;
                    tabControl1.SelectedIndex = 3;
                    listOfEventsForFinal = new List<int> { 1, 2 };
                    PlayOffFinal(listOfEventsForFinal, playOff_Inserter);
                    break;
            }            
        }

        //For creating the dress shape on the RESULTS tab.
        private void panel_Winner_Paint(object sender, PaintEventArgs e)
        {
            Point[] points =
            {
                new Point(120, 30),
                new Point(150, 60),
                new Point(180, 30),
                new Point(225, 45),
                new Point(270, 120),
                new Point(225, 150),
                new Point(210, 135),
                new Point(210, 270),
                new Point(90, 270),
                new Point(90, 135),
                new Point(75, 150),
                new Point(30, 120),
                new Point(75, 45),
                new Point(120, 30),
            };
            using (Graphics gw = this.panel_Winner.CreateGraphics())
            {
                Brush bw = new SolidBrush(winnerColor);
                gw.FillPolygon(bw, points);
            }
        }

        //A really ugly method that resets all variables, UI controls etc. to their default values.
        private void btnResetEverything_Click(object sender, EventArgs e)
        {
            DialogResult msgBoxResult = MessageBox.Show("Are you sure? This resets all your progress. If you want to for example export your configuration, you should do it before pressing this button.", null, MessageBoxButtons.YesNo, MessageBoxIcon.Question);            
            if (msgBoxResult == DialogResult.Yes)
            {
                btnAddFromQatar2022.Enabled = true;
                btnDeleteSelected.Enabled = true;
                btnAddCustomTeam.Enabled = true;
                textBoxCustomName.Enabled = true;
                rbpWhite.Enabled = true; rbpBlack.Enabled = true; rbpGray.Enabled = true; rbpBrown.Enabled = true;
                rbpRed.Enabled = true; rbpMaroon.Enabled = true; rbpYellow.Enabled = true; rbpOrange.Enabled = true;
                rbpGreen.Enabled = true; rbpLime.Enabled = true; rbpLightBlue.Enabled = true; rbpDarkBlue.Enabled = true;
                rbsWhite.Enabled = true; rbsBlack.Enabled = true; rbsGray.Enabled = true; rbsBrown.Enabled = true;
                rbsRed.Enabled = true; rbsMaroon.Enabled = true; rbsYellow.Enabled = true; rbsOrange.Enabled = true;
                rbsGreen.Enabled = true; rbsLime.Enabled = true; rbsLightBlue.Enabled = true; rbsDarkBlue.Enabled = true;
                rbpWhite.Enabled = true;
                btnClearAll.Enabled = true;
                btnImport.Enabled = true;
                btnAddA.Enabled = true; btnAddB.Enabled = true; btnAddC.Enabled = true; btnAddD.Enabled = true;
                btnAddE.Enabled = true; btnAddF.Enabled = true; btnAddG.Enabled = true; btnAddH.Enabled = true;
                btnUngroup.Enabled = true;
                btnAddRest.Enabled = true;
                listBoxA.Enabled = true; listBoxB.Enabled = true; listBoxC.Enabled = true; listBoxD.Enabled = true;
                listBoxE.Enabled = true; listBoxF.Enabled = true; listBoxG.Enabled = true; listBoxH.Enabled = true;
                btnFinishSetup.Enabled = true;
                listOfParticipants.Items.Clear();
                labelAmountOfCountries.Text = "Teams: 0 / 32";
                textBoxCustomName.Text = "";
                rbpWhite.Checked = false; rbpBlack.Checked = false; rbpGray.Checked = false; rbpBrown.Checked = false;
                rbpRed.Checked = false; rbpMaroon.Checked = false; rbpYellow.Checked = false; rbpOrange.Checked = false;
                rbpGreen.Checked = false; rbpLime.Checked = false; rbpLightBlue.Checked = false; rbpDarkBlue.Checked = false;
                rbsWhite.Checked = false; rbsBlack.Checked = false; rbsGray.Checked = false; rbsBrown.Checked = false;
                rbsRed.Checked = false; rbsMaroon.Checked = false; rbsYellow.Checked = false; rbsOrange.Checked = false;
                rbsGreen.Checked = false; rbsLime.Checked = false; rbsLightBlue.Checked = false; rbsDarkBlue.Checked = false;
                listBoxA.Items.Clear();
                listBoxB.Items.Clear();
                listBoxC.Items.Clear();
                listBoxD.Items.Clear();
                listBoxE.Items.Clear();
                listBoxF.Items.Clear();
                listBoxG.Items.Clear();
                listBoxH.Items.Clear();
                dgw_A.Rows.Clear();
                dgw_B.Rows.Clear();
                dgw_C.Rows.Clear();
                dgw_D.Rows.Clear();
                dgw_E.Rows.Clear();
                dgw_F.Rows.Clear();
                dgw_G.Rows.Clear();
                dgw_H.Rows.Clear();
                list_GroupA.Clear();
                list_GroupB.Clear();
                list_GroupC.Clear();
                list_GroupD.Clear();
                list_GroupE.Clear();
                list_GroupF.Clear();
                list_GroupG.Clear();
                list_GroupH.Clear();
                lblHomeTeam.Text = "";
                lblAwayTeam.Text = "";
                textBoxTeam1.Text = "";
                textBoxTeam2.Text = "";
                lbl_TypeOfTie.Text = "";
                dgw_ties.Rows.Clear();
                lbl_WhatToClick.Text = "";
                btn_tie_1.Text = "These";
                btn_tie_2.Text = "Are";
                btn_tie_3.Text = "Indeed";
                btn_tie_4.Text = "Buttons";
                ros1.Items.Clear(); ros2.Items.Clear(); ros3.Items.Clear(); ros4.Items.Clear();
                ros5.Items.Clear(); ros6.Items.Clear(); ros7.Items.Clear(); ros8.Items.Clear();
                quart1.Items.Clear(); quart2.Items.Clear(); quart3.Items.Clear(); quart4.Items.Clear();
                semi1.Items.Clear(); semi2.Items.Clear();
                thirdPlaceMatch.Items.Clear();
                finaleListBox.Items.Clear();
                btnStartPlayOffRounds.Text = "x";
                lblWinner.Text = "";
                labelSecondPlace.Text = "";
                labelThirdPlace.Text = "";
                homeColor = Color.DimGray;
                awayColor = Color.DimGray;
                winnerColor = Color.DimGray;
                panel_Dress1_Paint(this, null);
                panel_Dress2_Paint(this, null);
                panel_Winner_Paint(this, null);
                participants.Clear();
                whatRoundIsIt = 0;
                Array.Clear(orderOfButtons, 0, orderOfButtons.Length);
                isItPlayOff = false;
                Array.Clear(playOffArray, 0, playOffArray.Length);
                Array.Clear(quarterfinalArray, 0, quarterfinalArray.Length);
                Array.Clear(semifinalArray, 0, semifinalArray.Length);
                Array.Clear(finalArray, 0, finalArray.Length);
                Array.Clear(resultsArray, 0, resultsArray.Length);
                stepInSortGroups = 0;
                whatPlayOffRoundIsIt = "Round-of-16";
                tabControl1.SelectedIndex = 0;                
            }
            else
            {
                
            }
        }

        //Exits the app.
        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //For creating the dress shape of the home team on the MATCHES tab.
        private void panel_Dress1_Paint(object sender, PaintEventArgs e)
        {
            Point[] points =
            {
                new Point(120, 30),
                new Point(150, 60),
                new Point(180, 30),
                new Point(225, 45),
                new Point(270, 120),
                new Point(225, 150),
                new Point(210, 135),
                new Point(210, 270),
                new Point(90, 270),
                new Point(90, 135),
                new Point(75, 150),
                new Point(30, 120),
                new Point(75, 45),
                new Point(120, 30),
            };

            using (Graphics g1 = this.panel_Dress1.CreateGraphics())
            {
                Brush b1 = new SolidBrush(homeColor);
                g1.FillPolygon(b1, points);
            }
        }

        //For creating the dress shape of the away team on the MATCHES tab.
        private void panel_Dress2_Paint(object sender, PaintEventArgs e)
        {
            Point[] points =
            {
                new Point(120, 30),
                new Point(150, 60),
                new Point(180, 30),
                new Point(225, 45),
                new Point(270, 120),
                new Point(225, 150),
                new Point(210, 135),
                new Point(210, 270),
                new Point(90, 270),
                new Point(90, 135),
                new Point(75, 150),
                new Point(30, 120),
                new Point(75, 45),
                new Point(120, 30),
            };

            using (Graphics g2 = this.panel_Dress2.CreateGraphics())
            {
                Brush b2 = new SolidBrush(awayColor);
                g2.FillPolygon(b2, points);
            }
        }

        //Converts the String value from a CountryWithStats object into Color.
        public System.Drawing.Color TextToColor(string input)
        {   
            switch (input)
            {
                case "White":
                    return System.Drawing.Color.Gainsboro;
                case "Gray":
                    return System.Drawing.Color.Gray;
                case "Red":
                    return System.Drawing.Color.Red;
                case "Yellow":
                    return System.Drawing.Color.Yellow;
                case "Green":
                    return System.Drawing.Color.DarkGreen;
                case "LightBlue":
                    return System.Drawing.Color.LightBlue;
                case "Black":
                    return System.Drawing.Color.Black;
                case "Brown":
                    return System.Drawing.Color.Brown;
                case "Maroon":
                    return System.Drawing.Color.Maroon;
                case "Orange":
                    return System.Drawing.Color.Orange;
                case "Lime":
                    return System.Drawing.Color.Lime;
                case "DarkBlue":
                    return System.Drawing.Color.DarkBlue;
                default:
                    return System.Drawing.Color.DimGray;
            }
        }
    }
}
//V. Štodt