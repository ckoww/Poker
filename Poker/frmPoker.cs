using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        PictureBox[] pic = new PictureBox[5];
        int[] allPoker = new int[52];
        int[] playerPoker = new int[5];
        Random rand = new Random();

        int totalCredits = 10000; // 初始籌碼
        int currentBet = 0;      // 該局押注

        public frmPoker()
        {
            InitializeComponent();
            InitializePoker();
            txtTotalCredits.Text = totalCredits.ToString();
            txtTotalCredits.ReadOnly = true; // 禁止手動輸入總籌碼
            txtBetAmount.Text = "10";      // 預設下注
            btnDealCard.Enabled = false;// 一開始不能發牌，要先下注
            btnCheck.Enabled = false;
        }


        private void InitializePoker()
        {
            // 動態產生5張牌
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                pic[i].Enabled = false;
                pic[i].Visible = false;
                pic[i].Visible = true;

                // 將 pic 丟至到 grpPorker 內
                this.grpPoker.Controls.Add(pic[i]);
                pic[i].MouseClick += new MouseEventHandler(pic_Click);
            }
        }

        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
            
        }   

        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            
            this.lblResult.Text = "";
            // 先將牌面蓋掉
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
            }
            // 初始化52張牌
            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }
            // 洗牌
            Shuffle();

            await Task.Delay(500);
            // 發牌
            for (int i = 0; i < playerPoker.Length; i++)
            {
                playerPoker[i] = allPoker[i];                
            }

            this.ShowCards();

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Tag = "front";
                pic[i].Enabled = true;
            }
            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;

        }

        private void pic_Click(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            // 取得 pic 的索引值
            int index = int.Parse(pic.Name.Replace("pic", ""));
            int cardNum = playerPoker[index] + 1; // 取得玩家手牌的牌號
            // 如果 pic 的 Tag 為 back，則將顯示撲克牌
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage("pic" + (playerPoker[index] + 1));
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }


        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < allPoker.Length; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[i];
                allPoker[0] = temp;
            }
        }

        private Image GetImage(string name)
        {
            return Poker.Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int index = int.Parse(pic.Name.Replace("pic", "")); // 從 pic0、pic1...中取出索引
            int cardNum = playerPoker[index] + 1; // 取得玩家手牌的牌號

            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage("cardNum");
            }
            else {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }

        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5;
            for (int i = 0; i < pic.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    playerPoker[i] = allPoker[startIndex];
                    startIndex++;
                    pic[i].Image = GetImage("pic" + (playerPoker[i] + 1));
                    pic[i].Tag = "front";

                }
            }
            // 禁用所有牌的點擊事件
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = false;
            }

            this.btnChangeCard.Enabled = false;
            this.btnCheck.Enabled = true;

        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            // 計錄目前五張撲克牌的花色和點數的陣列
            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];
            // 將每張牌的顏色和點數分別存入 pokerColor 和 pokerPoint 陣列
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
            }
            int[] colorCount = new int[4];
            int[] pointCount = new int[13];
            for (int i = 0; i < 5; i++)
            {
                int color = pokerColor[i];
                int point = pokerPoint[i];
                colorCount[color]++;
                pointCount[point]++;
            }
            // 排序 colorCount 和 pointCount 由大到小
            Array.Sort(colorCount, colorList);
            Array.Reverse(colorCount);
            Array.Reverse(colorList);

            Array.Sort(pointCount, pointList);
            Array.Reverse(pointCount);
            Array.Reverse(pointList);

            // 判斷是否為同花
            bool isFlush = (colorCount[0] == 5);
            // 判斷是否為五張單張
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 &&
            pointCount[3] == 1 && pointCount[4] == 1);
            // 判斷是否為差四
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            // 判斷是否為大順
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) &&
            pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            // 判斷是否為同花大順
            bool isRoyalisFlush = isFlush && isRoyal;
            // 判斷是否為同花順
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            // 判斷是否為順子
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            // 判斷是否為鐵支
            bool isFourOfAKind = (pointCount[0] == 4);
            // 判斷是否為葫蘆
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            // 判斷是否為三條
            bool isThreeOfAKind = (pointCount[0] == 3 && pointCount[1] == 1);
            // 判斷是否為兩對
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            // 判斷是否為一對
            bool isOnePair = (pointCount[0] == 2 && pointCount[1] == 1);

            string result = "";
            int multiplier = 0;

            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 同花大順";
                multiplier = 250;
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
                multiplier = 50;
            }
            else if (isStraight)
            {
                result = "順子";
                multiplier = 4;
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 鐵支";
                multiplier = 25;
            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
                multiplier = 9;
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
                multiplier = 6;
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
                multiplier = 3;
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]},{pointList[1]} 兩對";
                multiplier = 2;
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
                multiplier = 1;
            }
            else
            {
                result = "雜牌";
                multiplier = 0;
            }

            lblResult.Text = result;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
            // --- 獎金結算與介面顯示 ---
            if (multiplier > 0)
            {
                int winAmount = currentBet * multiplier; // 計算獎金
                totalCredits += winAmount; // 將獎金加回總籌碼
                lblResult.Text = $"{result}，贏得 {winAmount}。請重新下注";
            }
            else
            {
                lblResult.Text = $"{result}，沒中獎。請重新下注";
            }

            // 更新畫面上的總籌碼顯示
            txtTotalCredits.Text = totalCredits.ToString();

            // 重要：重啟下注功能
            btnBet.Enabled = true;
            txtBetAmount.Enabled = true;
            btnDealCard.Enabled = false; // 下一局必須先按 btnBet 才能再按發牌
            btnCheck.Enabled = false;


        }
        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.btnDealCard.Enabled == false)
            {
                switch ((int)e.KeyChar)
                {
                    case 'q': // q鍵
                              // 同花大順
                        playerPoker[0] = 51;
                        playerPoker[1] = 47;
                        playerPoker[2] = 43;
                        playerPoker[3] = 39;
                        playerPoker[4] = 3;
                        break;
                    case 'w': // w鍵
                              // 同花順
                        playerPoker[0] = 37;
                        playerPoker[1] = 33;
                        playerPoker[2] = 29;
                        playerPoker[3] = 25;
                        playerPoker[4] = 21;
                        break;
                    case 'e': // e鍵
                              // 同花
                        playerPoker[0] = 50;
                        playerPoker[1] = 38;
                        playerPoker[2] = 34;
                        playerPoker[3] = 22;
                        playerPoker[4] = 18;
                        break;
                    case 'r': // r鍵
                              // 鐵支
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 38;
                        playerPoker[3] = 37;
                        playerPoker[4] = 36;
                        break;
                    case 't':  // t鍵
                               // 葫蘆
                        playerPoker[0] = 30;
                        playerPoker[1] = 29;
                        playerPoker[2] = 6;
                        playerPoker[3] = 5;
                        playerPoker[4] = 4;
                        break;
                    case 121:  // y鍵
                               // 三條
                        playerPoker[0] = 48;
                        playerPoker[1] = 39;
                        playerPoker[2] = 15;
                        playerPoker[3] = 14;
                        playerPoker[4] = 13;
                        break;
                }
                // 顯示五張撲克牌到桌面上
                ShowCards();
            }
            //MessageBox.Show("KeyPress: " + e.KeyChar);
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            // 1. 檢查輸入是否有效
            if (!int.TryParse(txtBetAmount.Text, out currentBet) || currentBet <= 0)
            {
                MessageBox.Show("請輸入正確的下注金額！");
                return;
            }

            // 2. 檢查籌碼
            if (currentBet > totalCredits)
            {
                MessageBox.Show("您的籌碼不足！");
                return;
            }

            // 3. 扣除籌碼並鎖定下注介面
            totalCredits -= currentBet;
            txtTotalCredits.Text = totalCredits.ToString();

            btnBet.Enabled = false;       // 下注後禁用下注按鈕
            txtBetAmount.Enabled = false; // 下注後禁用輸入框
            btnDealCard.Enabled = true;   // 下注成功，啟用發牌按鈕

            lblResult.Text = $"已下注 {currentBet}，請點擊發牌！";
        }
    }

 }

