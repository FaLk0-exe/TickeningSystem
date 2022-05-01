using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TickeningSystem
{
    public partial class UserForm : Form
    {
        Regex regex = new Regex(@"^[a-zA-Z0-9_]{5,}$");
        public UserForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(regex.IsMatch(textBox1.Text)&&regex.IsMatch(textBox2.Text))
            {
                if(comboBox1.SelectedIndex!=-1)
                {
                    using (Cinema c=new Cinema())
                    {
                        if(!c.User.Any(u=>u.Login==textBox1.Text))
                        {
                            c.User.Add(new User
                            {
                                Login = textBox1.Text,
                                Password = textBox2.Text,
                                Role = comboBox1.SelectedItem.ToString()
                            });
                            c.SaveChanges();
                            MessageBox.Show("OK!");
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Користувач з таким логіном вже зареєстрований у системі", "", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Оберіть роль користувача!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Вимоги до пароля та логіну:\n*Повинен бути більше 5 символів;\n*Допустимі символи:Латиниця, цифри та знак '_' ;","",MessageBoxButtons.YesNo,MessageBoxIcon.Error);
            }
        }
    }
}
