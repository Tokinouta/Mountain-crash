﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        PlayerGroup players = null;
        Mountain[] mountains = null;
        River[] rivers = null;
        Clinic[] clinics = null;
        Pit[] pits = null;

        Proprieter proprieter = null;
        Hat hat = null;
        Egg egg = null;
        Elf elf = null;
        Ozone ozone = null;

        //sec为秒计时，mini为分计时
        int TimeInSecond, TimeInMinute;
        bool isGenerated = false, isStarted = false;
        
        void CleanUp()
        {
            BattleField.Refresh();
            BattleField.Controls.Clear();
            BattleField.Visible = false;
            groupBox1.Visible = true;
            //groupBox2.Visible = true;
            Result.Visible = true;
            Result.Rows.Clear();
        }

        void FinishUp()
        {
            if (hat != null)
            {
                hat.TimerCountDown.Enabled = false;
            }
            if (elf != null)
            {
                elf.TimerOfProtection.Enabled = false;
                elf.TimerOfRecharge.Enabled = false;
            }
            if (egg != null)
            {
                egg.GettingIntoEarth.Enabled = false;
            }

            generation.Enabled = true;
            start.Enabled = false;
            pause.Enabled = false;
            isGenerated = false;
            isStarted = false;

            BattleField.Refresh();
            BattleField.Controls.Clear();
            BattleField.Visible = false;
            groupBox1.Visible = true;
            //groupBox2.Visible = true;
            Result.Visible = true;
            timer.Enabled = false;
            TimeInSecond = 0;
            gamingTime.Text = "000:00";
            timerForBattle.Enabled = false;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void Generation_Click(object sender, EventArgs e)
        {
            // horisontal: form width = max object width - 18
            // vertical: form height = max object height - 48
            // textBox1.Text = 3.ToString();
            //MountainNumber.Text = 1.ToString();
            //RiverNumber.Text = 1.ToString();
            //ClinicNumber.Text = 10.ToString();
            //PitNumber.Text = 10.ToString();
            //proprietorExists.Checked = true;
            //eggExists.Checked = true;
            //elfExists.Checked = true;
            //hatExists.Checked = true;
            //ozoneExists.Checked = true;
            //combatForceOptions.SelectedIndex = 1;
            //killOptions.SelectedIndex = 2;

            if (killOptions.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose kill options", "Warning: No choice of kill options");
                return;
            }
            if (combatForceOptions.SelectedIndex < 0)
            {
                MessageBox.Show("Please choose combat force options", "Warning: No choice of combat options");
                return;
            }
            Random random = new Random(Guid.NewGuid().GetHashCode());

            BattleField.Top = groupBox1.Top;
            BattleField.Left = groupBox1.Left;
            BattleField.Width = Width - groupBox1.Left - 10 - 10;
            BattleField.Height = Height - groupBox1.Top - 35 - 10;
            groupBox1.Visible = false;
            //groupBox2.Visible = false;
            Result.Visible = false;
            BattleField.Visible = true;
            Result.Visible = false;
            Result.Rows.Clear();

            #region player initialization
            players = new PlayerGroup();
            StreamReader PlayerName;
            try
            {
                PlayerName = new StreamReader(@"./PlayerName.txt");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(@"请确认PlayerName.txt和此程序在同一路径。", "未发现PlayerName.txt");
                CleanUp();
                return;
            }
            int order = 0;
            string nameTemp = "";
            while (!PlayerName.EndOfStream)
            {
                nameTemp = PlayerName.ReadToEnd();
            }
            string[] names = nameTemp.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length <= 1)
            {
                MessageBox.Show("请添加至少2名玩家。", "玩家太少啦");
                CleanUp();
                return;
            }
            Player.PlayerNumber = names.Length;
            Player.PlayerRemainedNumber = names.Length;
            foreach (var name in names)
            {
                var p = new Player(order, combatForceOptions.SelectedIndex, BattleField, (int)TeamNumber.Value)
                {
                    PlayerName = name
                };
                p.PlayerLabel.Text = $"{p.Team.ToString()} {p.PlayerName} {p.CombatForceLevel.ToString()}";
                players.AddPlayer(p);
                ++order;
            }
            PlayerName.Close();

            if (proprietorExists.Checked)
            {
                proprieter = new Proprieter(BattleField);
                players.AddPlayer(proprieter);
            }
            if (eggExists.Checked)
            {
                egg = new Egg(BattleField);
                players.AddPlayer(egg);
            }
            if (elfExists.Checked)
            {
                elf = new Elf(BattleField);
                players.AddPlayer(elf);
            }
            if (hatExists.Checked)
            {
                hat = new Hat(combatForceOptions.SelectedIndex, BattleField);
                players.AddPlayer(hat);
            }
            if (ozoneExists.Checked)
            {
                ozone = new Ozone(combatForceOptions.SelectedIndex, BattleField);
                players.AddPlayer(ozone);
            }
            #endregion

            int Temp;
            if (!int.TryParse(MountainNumber.Text, out Temp))
            {
                MessageBox.Show("Invalid mountain number");
                CleanUp();
                MountainNumber.Text = "山的数量";
                return;
            }
            else
            {
                Temp = Temp <= 0 ? 1 : Temp;
                mountains = new Mountain[Temp];
                double switchL = 5 * (Math.Atan(-Temp / 10) / 2 + Math.PI / 4);
                for (int i = 0; i < Temp; i++)
                {
                    int rand = random.Next(5);
                    mountains[i] = new Mountain(rand, switchL, BattleField);
                }
            }// Mountain
            if (!int.TryParse(RiverNumber.Text, out Temp))
            {
                MessageBox.Show("Invalid river number");
                CleanUp();
                RiverNumber.Text = "河的数量";
                return;
            }
            else
            {
                Temp = Temp <= 0 ? 1 : Temp;
                rivers = new River[Temp];
                double switchL = 5 * (Math.Atan(-Temp / 10) / 8 + Math.PI / 16);
                for (int i = 0; i < Temp; i++)
                {
                    rivers[i] = new River(switchL, BattleField);
                }
            }// River
            if (!int.TryParse(ClinicNumber.Text, out Temp))
            {
                MessageBox.Show("Invalid clinic number");
                CleanUp();
                ClinicNumber.Text = "医疗站数量";
                return;
            }
            else
            {
                Temp = Temp <= 0 ? 1 : Temp;
                clinics = new Clinic[Temp];
                Clinic.NumberOfClinic = Temp;
                for (int i = 0; i < Temp; i++)
                {
                    clinics[i] = new Clinic(10, 10, BattleField);
                }
            }// Clinic
            if (!int.TryParse(PitNumber.Text, out Temp))
            {
                MessageBox.Show("Invalid pit number");
                CleanUp();
                PitNumber.Text = "坑的数量";
                return;
            }
            else
            {
                Temp = Temp <= 0 ? 1 : Temp;
                pits = new Pit[Temp];
                for (int i = 0; i < Temp; i++)
                {
                    pits[i] = new Pit(i, 10, 10, BattleField);
                }
            }// Pit  

            isGenerated = true;
            playerRemained.Text = players.Count.ToString();
            Number.Text = Player.PlayerNumber.ToString();
            RandomGeneration.Enabled = false;
            generation.Enabled = false;
            start.Enabled = true;
            clear.Enabled = true;
        }

        private void start_Click(object sender, EventArgs e)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            if (players == null)
            {
                MessageBox.Show("Please initialize first", "Warning: No players");
                return;
            }
            if (!isGenerated)
            {
                MessageBox.Show("Please initialize first", "Warning: No initialization");
            }
            generation.Enabled = false;
            start.Enabled = false;
            pause.Enabled = true;
            pause.Text = "Pause";
            clear.Enabled = true;

            TimeInSecond = 0;
            TimeInMinute = 0;
            timer.Enabled = true;
            if (hat != null)
            {
                hat.TimerCountDown.Enabled = true;
            }
            if (elf != null)
            {
                //elf.TimerOfProtection.Enabled = true;
                elf.TimerOfRecharge.Enabled = true;
            }

            isStarted = true;
            timerForBattle.Enabled = true;
            UpdateSpeed.Enabled = true;
            //Timer3.Enabled = True
            //Timer4.Enabled = True
            //Timer5.Enabled = True
        }

        private void pause_Click(object sender, EventArgs e)
        {
            if (isStarted)
            {
                timer.Enabled = false;
                timerForBattle.Enabled = false;
                UpdateSpeed.Enabled = false;
                //Timer3.Enabled = False
                //Timer5.Enabled = False
                if (hat != null)
                {
                    hat.TimerCountDown.Enabled = false;
                }
                if (elf != null)
                {
                    elf.TimerOfProtection.Enabled = false;
                    elf.TimerOfRecharge.Enabled = false;
                }
                pause.Text = "Continue";
                isStarted = false;
            }
            else
            {
                //Timer2.Enabled = True
                //Timer3.Enabled = True
                timer.Enabled = true;
                timerForBattle.Enabled = true;
                UpdateSpeed.Enabled = true;
                if (hat != null)
                {
                    hat.TimerCountDown.Enabled = true;
                }
                //if (elf != null)
                //{
                //    elf.TimerOfRecharge.Enabled = true;
                //}
                pause.Text = "Pause";
                isStarted = true;
            }
        }
        
        private void Clear_Click(object sender, EventArgs e)
        {
            if (isStarted)
            {
                MessageBox.Show("Please do not clear", "Warning: Game running");
                return;
            }
            BattleField.Refresh();
            BattleField.Controls.Clear();
            BattleField.Visible = false;

            generation.Enabled = true;
            start.Enabled = false;
            pause.Enabled = false;
            isGenerated = false;
            
            groupBox1.Visible = true;
            //groupBox2.Visible = true;
            Result.Visible = true;
            timer.Enabled = false;
            timerForBattle.Enabled = false;
            UpdateSpeed.Enabled = true;
            TimeInSecond = 0;
            gamingTime.Text = "000:00";
            if (hat != null)
            {
                hat.TimerCountDown.Enabled = false;
            }
            if (elf != null)
            {
                elf.TimerOfProtection.Enabled = false;
                elf.TimerOfRecharge.Enabled = false;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            foreach (var player in players.PlayerList)
            {
                if (!player.IsAlive)
                {
                    continue;
                }
                player.Move(BattleField);
            }

            foreach (var player1 in players.PlayerList)
            {
                if (!player1.IsAlive)
                {
                    player1.UpdateColor(BattleField);
                    continue;
                }
                foreach (var mountain in mountains)
                {
                    player1.CollapsedMountain(mountain);
                    while (player1.Polygon.IsCover(mountain.Polygon))
                    {
                        player1.Move(BattleField);
                    }
                }
                foreach (var river in rivers)
                {
                    player1.CollapsedRiver(river);
                }
                foreach (var clinic in clinics)
                {
                    player1.CollapseClinic(clinic);
                }
                foreach (var pit in pits)
                {
                    player1.CollapsedPit(pit, pits);
                }
            }

            if (proprieter != null && proprieter.IsAlive)
            {
                proprieter.FingerGame(players.PlayerList);
                foreach (var player2 in players.PlayerList)
                {
                    if (proprieter != player2 && player2.Polygon.IsCover(proprieter.Polygon))
                    {
                        proprieter.Settle(TimeInSecond);
                        player2.Settle(TimeInSecond);
                        player2.UpdateColor(BattleField);
                    }
                }
            }

            if (egg != null && egg.IsAlive)
            {
                foreach (var player2 in players.PlayerList)
                {
                    if (player2 != egg && player2.IsAlive && !egg.IsInEarth)
                    {
                        egg.GetIntoEarth(player2);
                        if (egg.HitPoint > 0)
                        {
                            egg.Settle(TimeInSecond);
                        }
                    }
                }
            }

            if (ozone != null && ozone.IsAlive)
            {
                foreach (var player2 in players.PlayerList)
                {
                    if (player2 != ozone && player2.IsAlive)
                    {
                        ozone.Radius(player2);
                        player2.Settle(TimeInSecond);
                    }
                }
            }

            foreach (var player1 in players.PlayerList)
            { 
                foreach (var player2 in players.PlayerList)
                {
                    if (player1 == proprieter)
                    {
                        player1.Settle(TimeInSecond);
                        continue;
                    }
                    else
                    {
                        if (player1 != player2 && player2 != proprieter && player2.IsAlive && player1.IsAlive)
                        {
                            player1.Battle(player2, killOptions, TeamNumber);
                            player1.Settle(TimeInSecond);
                            player2.Settle(TimeInSecond);
                        }
                    }
                }
            }

            playerRemained.Text = Player.PlayerRemainedNumber.ToString();
            switch (TeamNumber.Value)
            {
                case 0:
                    if (Player.PlayerRemainedNumber <= 1 && isStarted)
                    {
                        Result.Rows.Clear();
                        players.PlayerList.Sort();
                        players.PlayerList[0].SurvivalTime = TimeInSecond;
                        players.PlayerList[0].SurvivalRank = 1;
                        foreach (var player in players.PlayerList)
                        {
                            Result.Rows.Add(player.ShowInfo());
                        }
                        FinishUp();
                    }
                    break;
                case 1:
                    bool isFinish = isStarted &&
                        ((proprieter == null) || (proprieter != null && !proprieter.IsAlive)) &&
                        ((egg == null) || (egg != null && !egg.IsAlive)) &&
                        ((elf == null) || (elf != null && !elf.IsAlive)) &&
                        ((hat == null) || (hat != null && !hat.IsAlive)) &&
                        ((ozone == null) || (ozone != null && !ozone.IsAlive));
                    if (isFinish)
                    {
                        Result.Rows.Clear();
                        players.PlayerList.Sort();
                        foreach (var player in players.PlayerList)
                        {
                            if (player.IsAlive)
                            {
                                player.SurvivalTime = TimeInSecond;
                                player.SurvivalRank = 1;
                            }
                            Result.Rows.Add(player.ShowInfo());
                        }
                        FinishUp();
                    }
                    break;
                default:
                    int teamWin;
                    var teamTemp =
                        from p in players.PlayerList
                        where p.IsAlive && p.IsPlayer
                        select p.Team;
                    Dictionary<int, int> dic = new Dictionary<int, int>();
                    if (teamTemp.Count() > 0)
                    {
                        foreach (var val in teamTemp)
                        {
                            //若字典中不存在该元素，则将该元素加入字典中
                            if (!dic.ContainsKey(val))
                            {
                                dic.Add(val, 1);
                            }
                            else    //若字典中存在该元素，则统计该元素在数组中出现的次数
                            {
                                dic[val] += 1;
                            }
                        }
                        if (dic.Count == 1)
                        {
                            //foreach (var key in dic.Keys)
                            //{
                            //    teamWin = dic[key];
                            //    break;
                            //}
                            Result.Rows.Clear();
                            players.PlayerList.Sort();
                            foreach (var player in players.PlayerList)
                            {
                                if (player.IsAlive)
                                {
                                    player.SurvivalTime = TimeInSecond;
                                    player.SurvivalRank = 1;
                                }
                                Result.Rows.Add(player.ShowInfo());
                            }
                            FinishUp();
                        }
                    }
                    else
                    {
                        Result.Rows.Clear();
                        foreach (var player in players.PlayerList)
                        {
                            if (player.IsAlive)
                            {
                                player.SurvivalTime = TimeInSecond;
                                player.SurvivalRank = 1;
                            }
                            Result.Rows.Add(player.ShowInfo());
                        }
                        FinishUp();
                    }
                    break;
            }
        }

        private void UpdateSpeed_Tick(object sender, EventArgs e)
        {
            foreach (var player in players.PlayerList)
            {
                player.UpdateSpeed();
            }
            players.UpdateBattleForceLevel(combatForceOptions);
        }

        private void Textbox_Click(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.SelectAll();
        }

        private void BattleField_SizeChanged(object sender, EventArgs e)
        {
            if (!isGenerated)
            {
                return;
            }
        }

        private void RandomGeneration_Click(object sender, EventArgs e)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            proprietorExists.Checked = random.NextDouble() < 0.5;
            eggExists.Checked = random.NextDouble() < 0.5; 
            elfExists.Checked = random.NextDouble() < 0.5; 
            hatExists.Checked = random.NextDouble() < 0.5; 
            ozoneExists.Checked = random.NextDouble() < 0.5;

            MountainNumber.Text = random.Next(1, 10).ToString();
            RiverNumber.Text = random.Next(1, 10).ToString();
            ClinicNumber.Text = random.Next(1, 20).ToString();
            PitNumber.Text = random.Next(1, 20).ToString();

            combatForceOptions.SelectedIndex = random.Next(3);
            killOptions.SelectedIndex = random.Next(4);
            TeamNumber.Value = random.Next(4);
        }

        private void TeamNumberPrompt_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "0：无队伍配置\n" +
                "1：齐心协力打NPC\n" +
                "2：2个队\n" +
                "3：3个队\n" +
                "NPC不参加队伍分配\n"+
                "玩家按PlayerName.txt里的顺序分队，从前到后，数够人数即成队",
                "选项说明");
        }

        private void BattleField_Paint(object sender, PaintEventArgs e)
        {
            Graphics DrawBarriers = e.Graphics;
            Pen DrawMountainPen = new Pen(Brushes.Black, 2);
            if (mountains != null)
            {
                foreach (var mount in mountains)
                {
                    DrawBarriers.DrawLine(DrawMountainPen, mount.StartPoint, mount.EndPoint);
                }
            }

            Pen DrawRiverPen = new Pen(Brushes.Azure, 2);
            if (rivers != null)
            {
                foreach (var river in rivers)
                {
                    DrawBarriers.DrawLine(DrawRiverPen, river.StartPoint, river.EndPoint);
                }
            }

            Pen DrawClinicPen = new Pen(Brushes.Chocolate, 2);
            Brush DrawClinicBrush = new SolidBrush(Color.Cornsilk);
            if (clinics != null)
            {
                foreach (var clinic in clinics)
                {
                    DrawBarriers.DrawRectangle(DrawClinicPen, clinic.Left, clinic.Top, clinic.Width, clinic.Height);
                    DrawBarriers.FillRectangle(DrawClinicBrush, clinic.Left, clinic.Top, clinic.Width, clinic.Height);
                }
            }

            Pen DrawPitPen = new Pen(Brushes.DarkSeaGreen, 2);
            Brush DrawPitBrush = new SolidBrush(Color.MintCream);
            if (pits != null)
            {
                foreach (var pit in pits)
                {
                    DrawBarriers.DrawRectangle(DrawPitPen, pit.Left, pit.Top, pit.Width, pit.Height);
                    DrawBarriers.FillRectangle(DrawPitBrush, pit.Left, pit.Top, pit.Width, pit.Height);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeInSecond++;
            TimeInMinute = Convert.ToInt32(Math.Floor(Convert.ToDouble(TimeInSecond) / 60));
            gamingTime.Text = $"{TimeInMinute:D3}:{TimeInSecond % 60:D2}";
        }
    }
}
