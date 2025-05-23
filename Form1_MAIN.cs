using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace project_oop
{
    public partial class Form1 : Form
    {
       
            // ประกาศตัวแปรสำหรับเล่นเพลงพื้นหลัง, เสียงยิง, และเสียงระเบิด
            WindowsMediaPlayer gameMedia;
            WindowsMediaPlayer shootgMedia;
            WindowsMediaPlayer explosion;

            // ประกาศอาร์เรย์สำหรับกระสุนของศัตรู, ดาว, กระสุนของผู้เล่น และ ศัตรู
            PictureBox[] enemiesMunition;
            int enemiesMunitionSpeed;

            PictureBox[] stars;
            int playerSpeed;
            int backgroundspeed;

            PictureBox[] munitions;
            int MunitionSpeed;

            PictureBox[] enemies;
            int enemiSpeed;
            int Collision;

            Random rnd;

            // ประกาศตัวแปรสำหรับคะแนน, เลเวล, ความยาก, สถานะของเกม
            int score;
            int level;
            int deficulty;
            bool pause;
            bool gameIsOver;

            public Form1()
            {
                // เรียกใช้งานเมธอด InitializeComponent() เพื่อเริ่มต้นการตั้งค่าภายในฟอร์ม
                InitializeComponent();
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                // สร้าง Label สำหรับแสดงคะแนน
                scorelbl = new Label();
                scorelbl.Location = new Point(10, 10);
                scorelbl.Size = new Size(100, 30);
                scorelbl.ForeColor = Color.White;
                scorelbl.Font = new Font("Arial", 14, FontStyle.Bold);
                scorelbl.Text = "Score: 00";
                this.Controls.Add(scorelbl);

                // สร้าง Label สำหรับแสดงเลเวล
                levellbl = new Label();
                levellbl.Location = new Point(10, 50);
                levellbl.Size = new Size(100, 30);
                levellbl.ForeColor = Color.White;
                levellbl.Font = new Font("Arial", 14, FontStyle.Bold);
                levellbl.Text = "Level: 01";
                this.Controls.Add(levellbl);

                // กำหนดค่าตัวแปรเริ่มต้นของเกม
                pause = false;
                gameIsOver = false;
                score = 0;
                level = 1;
                deficulty = 9;

                backgroundspeed = 4;
                playerSpeed = 4;
                enemiSpeed = 4;
                MunitionSpeed = 20;
                enemiesMunitionSpeed = 4;

                munitions = new PictureBox[3];


                // โหลดภาพกระสุนและศัตรู
                Image munition = Image.FromFile(@"asserts\munition.png");

                Image enemi1 = Image.FromFile("asserts\\E1.png");
                Image enemi2 = Image.FromFile("asserts\\E2.png");
                Image enemi3 = Image.FromFile("asserts\\E3.png");
                Image boss1 = Image.FromFile("asserts\\Boss1.png");
                Image boss2 = Image.FromFile("asserts\\Boss2.png");

                enemies = new PictureBox[10];

                // เริ่มต้นตั้งค่าภาพของศัตรู
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i] = new PictureBox();
                    enemies[i].Size = new Size(40, 40); // กำหนดขนาดของศัตรู
                    enemies[i].SizeMode = PictureBoxSizeMode.Zoom;
                    enemies[i].BorderStyle = BorderStyle.None;
                    enemies[i].Visible = false;
                    this.Controls.Add(enemies[i]);

                    // ตั้งตำแหน่งเริ่มต้นของศัตรูให้อยู่เหนือหน้าจอ
                    enemies[i].Location = new Point((i + 1) * 50, -50);
                }

                // กำหนดภาพให้กับศัตรูแต่ละตัว
                enemies[0].Image = boss1;
                enemies[1].Image = enemi2;
                enemies[2].Image = enemi3;
                enemies[3].Image = enemi3;
                enemies[4].Image = enemi1;
                enemies[5].Image = enemi3;
                enemies[6].Image = enemi2;
                enemies[7].Image = enemi3;
                enemies[8].Image = enemi2;
                enemies[9].Image = boss2;

                // เริ่มต้นการตั้งค่ากระสุนของผู้เล่น
                for (int i = 0; i < munitions.Length; i++)
                {
                    munitions[i] = new PictureBox();
                    munitions[i].Size = new Size(8, 8); // ตั้งขนาดของกระสุน
                    munitions[i].Image = munition; // กำหนดภาพของกระสุน
                    munitions[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    munitions[i].BorderStyle = BorderStyle.None;
                    this.Controls.Add(munitions[i]);
                }



                // สร้างอ็อบเจกต์ WindowsMediaPlayer
                gameMedia = new WindowsMediaPlayer();
                shootgMedia = new WindowsMediaPlayer();
                explosion = new WindowsMediaPlayer();

                // โหลดเพลงพื้นหลัง, เสียงยิง, และเสียงระเบิด
                gameMedia.URL = "songs\\GameSong.mp3";
                shootgMedia.URL = "songs\\shoot.mp3";
                explosion.URL = "songs\\boom.mp3";

                // ตั้งค่าการเล่นเพลง (การวนลูปและระดับเสียง)
                gameMedia.settings.setMode("loop", true);
                gameMedia.settings.volume = 50;
                shootgMedia.settings.volume = 1;
                explosion.settings.volume = 6;

                // สร้างดาวสำหรับพื้นหลัง
                stars = new PictureBox[15];

                rnd = new Random();

                // เริ่มต้นการตั้งค่ากระสุนของศัตรู
                enemiesMunition = new PictureBox[10];

                for (int i = 0; i < enemiesMunition.Length; i++)
                {
                    enemiesMunition[i] = new PictureBox();
                    enemiesMunition[i].Size = new Size(2, 25); // กำหนดขนาดของกระสุนศัตรู
                    enemiesMunition[i].Visible = false;
                    enemiesMunition[i].BackColor = Color.Yellow; // กำหนดสีของกระสุน
                    int x = rnd.Next(0, 10);
                    enemiesMunition[i].Location = new Point(enemies[x].Location.X, enemies[x].Location.Y - 20); // ตั้งตำแหน่งกระสุนศัตรูให้อยู่เหนือศัตรู
                    this.Controls.Add(enemiesMunition[i]);
                }

                // เริ่มเล่นเพลงพื้นหลัง
                gameMedia.controls.play();

                // สร้างดาวในพื้นหลังโดยสุ่มตำแหน่ง
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i] = new PictureBox();
                    stars[i].BorderStyle = BorderStyle.None;
                    stars[i].Location = new Point(rnd.Next(20, 580), rnd.Next(-10, 400)); // ตำแหน่งดาวที่สุ่ม
                    if (i % 2 == 1)
                    {
                        stars[i].Size = new Size(2, 2);
                        stars[i].BackColor = Color.Wheat; // ดาวสีอ่อน
                    }
                    else
                    {
                        stars[i].Size = new Size(3, 3);
                        stars[i].BackColor = Color.DarkGray; // ดาวสีเข้ม
                    }
                    this.Controls.Add(stars[i]);
                }
            }


            private void MoveBgTimer_Tick(object sender, EventArgs e)
            {
                // เคลื่อนที่ดาวในพื้นหลังที่มีความเร็วแตกต่างกัน
                for (int i = 0; i < stars.Length / 2; i++)
                {
                    // เลื่อนตำแหน่งดาวที่อยู่ในครึ่งแรกของอาร์เรย์
                    stars[i].Top += backgroundspeed;

                    // หากดาวเคลื่อนที่ออกจากหน้าจอด้านล่างให้เริ่มต้นใหม่ที่ด้านบน
                    if (stars[i].Top >= this.Height)
                    {
                        stars[i].Top = -stars[i].Height;
                    }
                }

                // เคลื่อนที่ดาวในพื้นหลังที่มีความเร็วช้ากว่าครึ่งแรก
                for (int i = stars.Length / 2; i < stars.Length; i++)
                {
                    // เลื่อนตำแหน่งดาวที่อยู่ในครึ่งหลังของอาร์เรย์
                    stars[i].Top += backgroundspeed - 2;  // ลดความเร็วลงเล็กน้อย

                    // หากดาวเคลื่อนที่ออกจากหน้าจอด้านล่างให้เริ่มต้นใหม่ที่ด้านบน
                    if (stars[i].Top >= this.Height)
                    {
                        stars[i].Top = -stars[i].Height;
                    }
                }
            }

            // การเคลื่อนที่ไปทางซ้าย
            private void LeftMoveTimer_Tick(object sender, EventArgs e)
            {
                if (Player.Left > 10)  // ตรวจสอบว่า Player ยังไม่ถึงขอบซ้ายของหน้าจอ
                {
                    Player.Left -= playerSpeed;  // เลื่อน Player ไปทางซ้าย
                }
            }

            // การเคลื่อนที่ไปทางขวา
            private void RightMoveTimer_Tick(object sender, EventArgs e)
            {
                if (Player.Right < 580)  // ตรวจสอบว่า Player ยังไม่ถึงขอบขวาของหน้าจอ
                {
                    Player.Left += playerSpeed;  // เลื่อน Player ไปทางขวา
                }
            }

            // การเคลื่อนที่ไปข้างล่าง
            private void DownMoveTimer_Tick(object sender, EventArgs e)
            {
                if (Player.Top < 260)  // ตรวจสอบว่า Player ยังไม่ถึงขอบล่างของหน้าจอ
                {
                    Player.Top += playerSpeed;  // เลื่อน Player ลง
                }
            }

            // การเคลื่อนที่ไปข้างบน
            private void UpMoveTimer_Tick(object sender, EventArgs e)
            {
                if (Player.Top > 10)   // ตรวจสอบว่า Player ยังไม่ถึงขอบบนของหน้าจอ
                {
                    Player.Top -= playerSpeed;  // เลื่อน Player ขึ้น
                }
            }

            // เมื่อกดปุ่มคีย์ลง
            private void Form1_KeyDown(object sender, KeyEventArgs e)
            {
                if (!pause)  // ถ้าเกมไม่ได้อยู่ในสถานะหยุดชั่วคราว
                {
                    if (e.KeyCode == Keys.Right)
                    {
                        RightMoveTimer.Start();  // เริ่มการเคลื่อนที่ไปทางขวา
                    }
                    if (e.KeyCode == Keys.Left)
                    {
                        LeftMoveTimer.Start();  // เริ่มการเคลื่อนที่ไปทางซ้าย
                    }
                    if (e.KeyCode == Keys.Down)
                    {
                        DownMoveTimer.Start();  // เริ่มการเคลื่อนที่ลง
                    }
                    if (e.KeyCode == Keys.Up)
                    {
                        UpMoveTimer.Start();  // เริ่มการเคลื่อนที่ขึ้น
                    }
                }
            }


            // เมื่อปล่อยปุ่มคีย์
            private void Form1_KeyUp(object sender, KeyEventArgs e)
            {
                // หยุดการเคลื่อนที่ทั้งหมดเมื่อปล่อยปุ่ม
                RightMoveTimer.Stop();
                LeftMoveTimer.Stop();
                DownMoveTimer.Stop();
                UpMoveTimer.Stop();

                // ตรวจสอบว่าเมื่อกดปุ่ม Space ให้หยุดหรือเริ่มเกมใหม่
                if (e.KeyCode == Keys.Space)
                {
                    if (!gameIsOver)  // หากเกมยังไม่จบ
                    {
                        if (pause)  // ถ้าเกมอยู่ในสถานะหยุดชั่วคราว
                        {
                            StartTimers();  // เริ่มทามเมอร์ทั้งหมด
                            Label.Visible = false;  // ซ่อนข้อความ PAUSED
                            gameMedia.controls.play();  // เริ่มเล่นเพลงพื้นหลัง
                            pause = false;  // ตั้งสถานะเกมเป็นไม่หยุดชั่วคราว
                        }
                        else  // ถ้าเกมไม่ได้อยู่ในสถานะหยุด
                        {


                            // ตั้งตำแหน่งข้อความ PAUSED ให้อยู่กลางหน้าจอ
                            Label.Location = new Point(this.Width / 2 - 120, 150);
                            Label.Text = "PAUSED";  // แสดงข้อความ "PAUSED"
                            Label.Visible = true;  // แสดงข้อความ
                            gameMedia.controls.pause();  // หยุดเพลงพื้นหลัง
                            StopTimers();  // หยุดทามเมอร์ทั้งหมด
                            pause = true;  // ตั้งสถานะเกมเป็นหยุดชั่วคราว
                        }
                    }
                }
            }

            private void StartTimers()
            {
                // เริ่มต้นการทำงานของทามเมอร์ทั้งหมดที่จำเป็นสำหรับการเล่นเกม
                RightMoveTimer.Start();  // เริ่มการเคลื่อนที่ไปทางขวา
                LeftMoveTimer.Start();  // เริ่มการเคลื่อนที่ไปทางซ้าย
                DownMoveTimer.Start();  // เริ่มการเคลื่อนที่ลง
                UpMoveTimer.Start();    // เริ่มการเคลื่อนที่ขึ้น
            }

            private void StopTimers()
            {
                // หยุดทามเมอร์ทั้งหมดเมื่อเกมหยุดชั่วคราวหรือเกมจบ
                RightMoveTimer.Stop();  // หยุดการเคลื่อนที่ไปทางขวา
                LeftMoveTimer.Stop();   // หยุดการเคลื่อนที่ไปทางซ้าย
                DownMoveTimer.Stop();   // หยุดการเคลื่อนที่ลง
                UpMoveTimer.Stop();     // หยุดการเคลื่อนที่ขึ้น
            }

            private void MoveMunitionTimer_Tick(object sender, EventArgs e)
            {
                shootgMedia.controls.play();  // เล่นเสียงเมื่อมีการยิงกระสุน
                for (int i = 0; i < munitions.Length; i++)
                {
                    if (munitions[i].Top > 0)
                    {
                        munitions[i].Visible = true;  // แสดงกระสุนที่เคลื่อนที่บนหน้าจอ
                        munitions[i].Top -= MunitionSpeed;  // ลดตำแหน่งกระสุนลงตามความเร็ว

                    }
                    else
                    {
                        munitions[i].Visible = false;  // ซ่อนกระสุนที่ออกนอกหน้าจอ
                        munitions[i].Location = new Point(Player.Location.X + 20, Player.Location.Y - i * 30);  // ตั้งตำแหน่งใหม่ของกระสุนตามผู้เล่น
                    }
                }
            }

            private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
            {
                // เคลื่อนที่ศัตรูตามความเร็วที่กำหนด
                MoveEnemies(enemies, enemiSpeed);
            }

            private void MoveEnemies(PictureBox[] array, int speed)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Visible = true;  // แสดงศัตรูบนหน้าจอ
                    array[i].Top += speed;  // เคลื่อนที่ศัตรูลงตามความเร็ว

                    // หากศัตรูเคลื่อนออกจากหน้าจอ
                    if (array[i].Top > this.Height)
                    {
                        // Reset ตำแหน่งศัตรูที่ด้านบนหน้าจอ
                        array[i].Location = new Point((i + 1) * 50, -200);
                    }


                    void Collision()
                    {
                        for (int j = 0; j < enemies.Length; j++)
                        {
                            // ตรวจสอบการชนระหว่างกระสุนและศัตรู
                            if (munitions[0].Bounds.IntersectsWith(enemies[j].Bounds) ||
                                munitions[1].Bounds.IntersectsWith(enemies[j].Bounds) ||
                                munitions[2].Bounds.IntersectsWith(enemies[j].Bounds))
                            {
                                explosion.controls.play();  // เล่นเสียงระเบิด
                                score += 1;  // เพิ่มคะแนน
                                scorelbl.Text = (score < 10) ? "0" + score.ToString() : score.ToString();

                                if (score % 30 == 0)
                                {
                                    level += 1;  // เพิ่มระดับเกม
                                    levellbl.Text = (level < 10) ? "0" + level.ToString() : level.ToString();

                                    if (enemiSpeed <= 10 && enemiesMunitionSpeed <= 10 && deficulty >= 0)
                                    {
                                        deficulty--;  // ทำให้เกมมีความยากเพิ่มขึ้น
                                        enemiSpeed++;  // เพิ่มความเร็วของศัตรู
                                        enemiesMunitionSpeed++;  // เพิ่มความเร็วของกระสุนศัตรู
                                    }

                                    if (level == 10)  // หากไปถึงระดับสุดท้าย
                                    {
                                        GameOver("NICE DONE");  // แสดงข้อความ Game Over
                                    }
                                }
                                enemies[j].Location = new Point((j + 1) * 50, -100);  // รีเซ็ตตำแหน่งศัตรูใหม่
                            }

                            // ตรวจสอบการชนระหว่างผู้เล่นและศัตรู
                            if (Player.Bounds.IntersectsWith(enemies[j].Bounds))
                            {
                                explosion.settings.volume = 30;  // ตั้งค่าความดังของเสียง
                                explosion.controls.play();  // เล่นเสียงระเบิด
                                Player.Visible = false;  // ซ่อนผู้เล่น
                                GameOver("Game Over");  // แสดงข้อความ Game Over
                            }
                        }
                    }

                    void GameOver(string str)
                    {
                        // แสดงข้อความ Game Over และปุ่มเล่นใหม่และออกจากเกม
                        Label.Text = str;
                        Label.Location = new Point(120, 120);
                        Label.Visible = true;
                        ReplayBtn.Visible = true;
                        ExitBtn.Visible = true;

                        StopTimers();  // หยุดการทำงานของทามเมอร์ทั้งหมด
                        gameMedia.controls.stop();  // หยุดการเล่นเพลงพื้นหลัง
                    }

                    void StopTimers()
                    {
                        // หยุดทามเมอร์ทั้งหมดที่ใช้ในการเล่นเกม
                        MoveBgTimer.Stop();
                        MoveEnemiesTimer.Stop();
                        MoveMunitionTimer.Stop();
                        EnemiesMunitionTimer.Stop();
                    }

                    void StartTimers()
                    {
                        // เริ่มทามเมอร์ทั้งหมดที่จำเป็นสำหรับการเล่นเกม
                        MoveBgTimer.Start();
                        MoveEnemiesTimer.Start();
                        MoveMunitionTimer.Start();
                        EnemiesMunitionTimer.Start();
                    }
                }
            }


            private void EnemiesMunitionTimer_Tick(object sender, EventArgs e)
            {
                // สำหรับการเคลื่อนที่ของกระสุนของศัตรู
                for (int i = 0; i < (enemiesMunition.Length - deficulty); i++)
                {
                    if (enemiesMunition[i].Top < this.Height)  // หากกระสุนยังไม่ถึงด้านล่างของหน้าจอ
                    {
                        enemiesMunition[i].Visible = true;  // ทำให้กระสุนมองเห็น
                        enemiesMunition[i].Top += enemiesMunitionSpeed;  // เคลื่อนที่กระสุนลงด้านล่าง

                        CollisionWithEnemisMunition();  // ตรวจสอบการชนระหว่างกระสุนศัตรูและผู้เล่น
                    }
                    else
                    {
                        enemiesMunition[i].Visible = false;  // ซ่อนกระสุนเมื่อมันออกจากหน้าจอ
                        int x = rnd.Next(0, 10);  // สุ่มตำแหน่งของศัตรูที่ยิงกระสุน
                        enemiesMunition[i].Location = new Point(enemies[x].Location.X + 20, enemies[x].Location.Y + 30);  // ตั้งตำแหน่งใหม่ให้กระสุนที่ศัตรู
                    }
                }
            }




            private void CollisionWithEnemisMunition()
            {
                // ตรวจสอบการชนระหว่างกระสุนของศัตรูและผู้เล่น
                for (int i = 0; i < enemiesMunition.Length; i++)
                {
                    if (enemiesMunition[i].Bounds.IntersectsWith(Player.Bounds))  // ถ้ากระสุนของศัตรูชนกับผู้เล่น
                    {
                        enemiesMunition[i].Visible = false;  // ซ่อนกระสุนที่ชน
                        explosion.settings.volume = 30;  // ตั้งค่าความดังของเสียงระเบิด
                        explosion.controls.play();  // เล่นเสียงระเบิด
                        Player.Visible = false;  // ซ่อนผู้เล่นเมื่อถูกชน
                    }
                }
            }

            private void ExitBtn_Click(object sender, EventArgs e)
            {
                // เมื่อกดปุ่ม Exit จะทำการออกจากโปรแกรม
                Environment.Exit(1);  // ออกจากแอปพลิเคชัน
            }




            private void ReplayBtn_Click(object sender, EventArgs e)
            {
                // เมื่อกดปุ่ม Replay จะรีเซ็ตเกมใหม่
                this.Controls.Clear();  // เคลียร์ทั้งหมดในฟอร์ม
                InitializeComponent();  // เรียกฟังก์ชันเริ่มต้นใหม่
                Form1_Load(e, e);  // เรียกฟังก์ชัน Load ใหม่เพื่อโหลดคอนโทรลทั้งหมด
            }
        }
    }
