using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        List<string> userList = new List<string>();
        List<Friends> friendsList = new List<Friends>();
        List<string> alreadyFriends = new List<string>();
        List<string> notFriends = new List<string>();
        Dictionary<string, int> mutualFriends = new Dictionary<string, int>();
        Dictionary<string, int> suggestedRanking = new Dictionary<string, int>();
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseUp += panel1_MouseUp;
            panel1.MouseMove += panel1_MouseMove;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += new DrawItemEventHandler(listBox1_DrawItem);
        }
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Color.Black);
            e.DrawBackground();
            e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(userList.Contains(textBox1.Text))
            {
                alreadyFriends.Clear();
                notFriends.Clear();
                dataGridView2.Rows.Clear();
                label1.Text = textBox1.Text;
                listBox2.Items.Clear();
                for(int x = 0;x < friendsList.Count;x++)
                {
                    if (label1.Text == friendsList[x].User)
                    {
                        listBox2.Items.Add(friendsList[x].Friend);
                    }
                }
                alreadyFriends = getAlreadyFriends(label1.Text, friendsList);
                notFriends = getNotFriends(alreadyFriends, userList);
                mutualFriends = getMutualFriends(alreadyFriends, friendsList, notFriends);
                suggestedRanking = getSuggestedRanking(mutualFriends);
                foreach (var friend in suggestedRanking)
                {
                    if(label1.Text != friend.Key)
                        dataGridView2.Rows.Add(friend.Key, friend.Value);
                }
            }
            else
            {
                userList.Add(textBox1.Text);
                listBox1.Items.Add(textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || label1.Text == "No User")
            {

            }
            else
            {
                alreadyFriends.Clear();
                notFriends.Clear();
                dataGridView2.Rows.Clear();
                listBox2.Items.Clear();
                friendsList.Add(new Friends(label1.Text, listBox1.SelectedItem.ToString()));
                friendsList.Add(new Friends(listBox1.SelectedItem.ToString(), label1.Text));
                button2.Enabled = false;
                for (int x = 0; x < friendsList.Count; x++)
                {
                    if (label1.Text == friendsList[x].User)
                        listBox2.Items.Add(friendsList[x].Friend);
                }
                alreadyFriends = getAlreadyFriends(label1.Text, friendsList);
                notFriends = getNotFriends(alreadyFriends, userList);
                mutualFriends = getMutualFriends(alreadyFriends, friendsList, notFriends);
                suggestedRanking = getSuggestedRanking(mutualFriends);
                foreach (var friend in suggestedRanking)
                {
                    if (label1.Text != friend.Key)
                        dataGridView2.Rows.Add(friend.Key, friend.Value);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(checkFriend(label1.Text,listBox1.SelectedItem.ToString(), friendsList) || label1.Text == listBox1.SelectedItem.ToString())
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }
        }

        public List<string> getNotFriends(List<string> AlreadyFriends, List<string> UserList)
        {
            List<string> NotFriends = new List<string>();
            for (int x = 0; x < UserList.Count; x++)
            {
                if (!AlreadyFriends.Contains(UserList[x]))
                {
                    NotFriends.Add(UserList[x]);
                }
            }
            return NotFriends;
        }

        public List<string> getAlreadyFriends(string User, List<Friends> FriendsList)
        {
            List<string> AlreadyFriends = new List<string>();
            for (int x = 0; x < FriendsList.Count; x++)
            {
                if (User == FriendsList[x].User)
                {
                    AlreadyFriends.Add(FriendsList[x].Friend);
                }
            }
            return AlreadyFriends;
        }
        public Dictionary<string, int> getSuggestedRanking(Dictionary<string, int> MutualFriends)
        {
            int[] mutual = new int[MutualFriends.Count];
            string[] name = new string[MutualFriends.Count];
            Dictionary<string, int> sortedFriends = new Dictionary<string, int>();
            int index = 0;
            foreach (var prod in MutualFriends)
            {
                mutual[index] = prod.Value;
                name[index] = prod.Key;
                index++;
            }
            for (int x = 0; x < MutualFriends.Count; x++)
            {
                int minimum = x;
                for (int y = x; y < MutualFriends.Count; y++)
                {
                    if (mutual[minimum] < mutual[y])
                    {
                        minimum = y;
                    }
                }
                int temp = mutual[x];
                mutual[x] = mutual[minimum];
                mutual[minimum] = temp;
                string tempName = name[x];
                name[x] = name[minimum];
                name[minimum] = tempName;
            }
            for (int x = 0; x < MutualFriends.Count; x++)
            {
                sortedFriends.Add(name[x], mutual[x]);
            }
            return sortedFriends;
        }

        public Dictionary<string, int> getMutualFriends(List<string> alreadyFriends, List<Friends> FriendsList, List<string> NotFriends)
        {
            Dictionary<string, int> mutualFriends = new Dictionary<string, int>();
            List<string> notFriendFriends = new List<string>();
            for(int x = 0;x < NotFriends.Count;x++)
            {
                notFriendFriends.Clear();
                int mutualCount = 0;
                for (int y = 0; y < FriendsList.Count; y++)
                {
                    if(NotFriends[x] == FriendsList[y].User)
                    {
                        notFriendFriends.Add(FriendsList[y].Friend);
                    }
                }
                for(int w = 0;w < notFriendFriends.Count; w++)
                {
                    if (alreadyFriends.Contains(notFriendFriends[w]))
                        mutualCount++;
                }
                mutualFriends.Add(NotFriends[x], mutualCount);

            }
            return mutualFriends;
        }

        public bool checkFriend(string CurrentUser, string SelectedUser, List<Friends> FriendList)
        {
            bool AlreadyFriend = false;
            for(int x = 0;x < FriendList.Count;x++)
            {
                if(CurrentUser == FriendList[x].User)
                {
                    if(SelectedUser == FriendList[x].Friend)
                    {
                        AlreadyFriend = true;
                        break;
                    }
                    else
                    {
                        AlreadyFriend =  false;
                    }
                }
            }
            return AlreadyFriend;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void DeleteUser()
        {
            int friendCount = 0;
            for (int x = 0; x < friendsList.Count; x++)
            {
                if (friendsList[x].User == listBox1.SelectedItem.ToString())
                {
                    friendCount++;
                }
            }
            for (int x = 0; x < friendCount; x++)
            {
                for (int y = 0; y < friendsList.Count; y++)
                {
                    if (friendsList[y].User == listBox1.SelectedItem.ToString())
                    {
                        friendsList.RemoveAt(y);
                    }
                }
                for (int w = 0; w < friendsList.Count; w++)
                {
                    if (friendsList[w].Friend == listBox1.SelectedItem.ToString())
                    {
                        friendsList.RemoveAt(w);
                    }
                }
            }
            dataGridView2.Rows.Clear();
            userList.Remove(listBox1.SelectedItem.ToString());
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            alreadyFriends.Clear();
            notFriends.Clear();
            for (int x = 0; x < userList.Count; x++)
            {
                listBox1.Items.Add(userList[x]);
            }
            for (int x = 0; x < friendsList.Count; x++)
            {
                if (label1.Text == friendsList[x].User)
                    listBox2.Items.Add(friendsList[x].Friend);
            }
            alreadyFriends = getAlreadyFriends(label1.Text, friendsList);
            notFriends = getNotFriends(alreadyFriends, userList);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1 || label1.Text == "No User")
            {
            }
            else if(label1.Text == listBox1.SelectedItem.ToString())
            {
                label1.Text = "No User";
                DeleteUser();
            }
            else
            {
                DeleteUser();
                mutualFriends = getMutualFriends(alreadyFriends, friendsList, notFriends);
                suggestedRanking = getSuggestedRanking(mutualFriends);
                foreach (var friend in suggestedRanking)
                {
                    if (label1.Text != friend.Key)
                        dataGridView2.Rows.Add(friend.Key, friend.Value);
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    public class Friends
    {
        public string User;
        public string Friend;
        
        public Friends(string User, string Friend)
        {
            this.User = User;
            this.Friend = Friend;
        }
    }
}
