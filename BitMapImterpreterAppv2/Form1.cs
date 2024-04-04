using Newtonsoft.Json;

namespace BitMapImterpreterAppv2
{
    public partial class Form1 : Form
    {
        CodeBase CB = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (ParseINST())
                CB.Run();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            string FilePath = GetFilePathOpen();
            if (FilePath != "Err")
                if (File.Exists(FilePath))
                    CB = JsonConvert.DeserializeObject<CodeBase>(File.ReadAllText(FilePath));
            MainTextBox.Text = CB.GetAllText();
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            SaveToCB();

            string FilePath = GetFilePathSave();
            if (FilePath != "Err")
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(CB, Formatting.Indented));
        }

        private void ParseInstructions_Click(object sender, EventArgs e)
        {
            SaveToCB();
            ParseINST();
        }

        bool ParseINST()
        {
            if (BitmapInterpreter.CheckSyntax(MainTextBox.Text))
            {
                Status.Text = "Sucsses";
                return true;
            }
            else
            {
                Status.Text = "Failed";
                return false;
            }
        }

        void SaveToCB()
        {
            CB.SaveToCB(MainTextBox.Text);
        }

        string GetFilePathOpen()
        {
            DialogResult DR = openFileDialog1.ShowDialog(this);
            if (DR == DialogResult.OK)
                return openFileDialog1.FileName;
            else
                return "Err";
        }
        string GetFilePathSave()
        {
            DialogResult DR = saveFileDialog1.ShowDialog(this);
            if (DR == DialogResult.OK)
                return saveFileDialog1.FileName;
            else
                return "Err";
        }
    }
}