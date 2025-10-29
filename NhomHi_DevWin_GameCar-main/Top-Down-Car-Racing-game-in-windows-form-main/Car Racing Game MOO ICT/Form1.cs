using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;



namespace Car_Racing_Game_MOO_ICT
{
    public partial class Form1
    {

        int roadSpeed;// Tốc độ di chuyển của đường
        int trafficSpeed; // Tốc độ di chuyển của xe AI random ra
        int playerSpeed = 12; // Tốc độ di chuyển của xe người chơi
        int score; // điểm số
        int carImage;   // biến để chọn hình ảnh xe AI
        int currentLevel = 1;   // Biến theo dõi level hiện tại
        int highScore = 0;      // Biến lưu điểm cao nhất
        bool isMusicOn = true;  // Biến theo dõi trạng thái nhạc nền
        string playerName = ""; // Tên người chơi
        string leaderboardFile = "leaderboard.txt"; //  Tên file bảng xếp hạng




        Random rand = new Random(); // Tạo đối tượng Random để sử dụng trong toàn lớp
        Random carPosition = new Random(); // Tạo đối tượng Random để chọn vị trí xe AI

        bool goleft, goright;


        public Form1()
        {
            InitializeComponent(); // Hàm của winform khởi tạo giao diện
            //this.KeyPreview = true;
            //this.KeyDown += keyisdown;
            //this.KeyUp += keyisup;
        }




        private void gameTimerEvent(object sender, EventArgs e) 
        {

            score++;
            txtScore.Text = "Điểm: " + score; // Cập nhật điểm số trên giao diện



            if (goleft == true && player.Left > 10)
            {
                player.Left -= playerSpeed; // Di chuyển xe người chơi sang trái
            } 
            if (goright == true && player.Left < 540)
            {
                player.Left += playerSpeed; // Di chuyển xe người chơi sang phải
            }
            // Giới hạn cho xe không ra khỏi form  

            roadTrack1.Top += roadSpeed; // Di chuyển tấm ảnh đường 1 xuống dưới
            roadTrack2.Top += roadSpeed;// Di chuyển tấm ảnh đường 2 xuống dưới

            if (roadTrack2.Top > 519)
            {
                roadTrack2.Top = -519;
            }
            if (roadTrack1.Top > 519)
            {
                roadTrack1.Top = -519;
            } // Tạo hiệu ứng tấm ảnh đường chạy liên tục, ảnh trôi ra khỏi khung thì cập nhật lại 

            AI1.Top += trafficSpeed; // Làm cho xe vật cản 1 di chuyển theo chiều dọc ( cập nhật tung độ )
            AI2.Top += trafficSpeed; // Làm cho xe vật cản 2 di chuyển theo chiều dọc ( cập nhật tung độ )


            if (AI1.Top > 530) // Kiểm tra nếu xe vật cản 1 ra khỏi khung hình
            {
                changeAIcars(AI1); // Gọi hàm thay đổi hình ảnh và vị trí hoành độ của xe AI1
            }

            if (AI2.Top > 530) // Kiểm tra nếu xe vật cản 2 ra khỏi khung hình
            {
                changeAIcars(AI2); // Gọi hàm thay đổi hình ảnh và vị trí hoành độ của xe AI1
            }


            // player là tên bức ảnh xe người chơi, Bounds là thuộc tính lấy vùng bao quanh ảnh, IntersectsWith là hàm kiểm tra va chạm giữa 2 vùng bao quanh
            // Tóm lại là kiểm tra va chạm giữa xe người chơi và 2 xe AI, thông qua việc kiểm tra vùng bao quanh của chúng có giao nhau hay không
            if (player.Bounds.IntersectsWith(AI1.Bounds) || player.Bounds.IntersectsWith(AI2.Bounds))
            {
                gameOver();
            }

            if (score > 40 && score <= 200)
            {
                txtLevel.Text = "Level 1";

            } // Kiểm tra và cập nhật level dựa trên điểm số


            if (score > 200 && score <= 500)
            {
                roadSpeed = 20;
                trafficSpeed = 22;
                txtLevel.Text = "Level 2";
                currentLevel = 2;
                ChangeCarImage(currentLevel);
            } // Nâng cấp level và sơn xe khi điểm số vượt quá 500 

            if (score > 500)
            {
                trafficSpeed = 27;
                roadSpeed = 25;
                txtLevel.Text = "Level 3";
                currentLevel = 3;
                ChangeCarImage(currentLevel);
            } // Nâng cấp level và sơn xe khi điểm số vượt quá 1500

            coin.Top += trafficSpeed; // Cho coin di chuyển trôi xuống theo chiều dọc
            if (coin.Top > 530)
            {
                changeItemPosition(coin);
            } // Nếu coin trôi ra khỏi khung thì đổi vị trí mới

            // Nếu ăn coin
            if (player.Bounds.IntersectsWith(coin.Bounds))
            {
                score += 50;
                txtScore.Text = "Điểm: " + score;
                changeItemPosition(coin); // Đổi vị trí mới cho coin
                System.Media.SystemSounds.Asterisk.Play();
            }

            // Xử lý vật phẩm slow chỉ hoạt động từ Level 2 trở lên
            if (currentLevel >= 2)
            {
                // Cho vật phẩm rơi xuống
                if (lowSpeed.Visible)
                    lowSpeed.Top += trafficSpeed; // Làm cho vật phẩm di chuyển trôi xuống theo chiều dọc

                // Nếu ra khỏi màn hình hoặc chưa hiển thị thì có 30% cơ hội spawn lại
                if (!lowSpeed.Visible || lowSpeed.Top > 530)
                {
                    lowSpeed.Visible = false;
                    if (rand.Next(1, 101) <= 30) // Ngẫu nhiên từ 1 đến 100, nếu ≤30 (30% cơ hội)
                    {
                        changeItemPosition(lowSpeed); // Đổi vị trí mới cho vật phẩm
                        lowSpeed.Visible = true;
                    }
                }

                //  Va chạm slow, giảm tốc trong 10 giây
                if (lowSpeed.Visible && player.Bounds.IntersectsWith(lowSpeed.Bounds))
                {
                    lowSpeed.Visible = false;

                    // Huỷ task cũ nếu có
                    speedResetToken?.Cancel(); // speedResetToken? là đang có tính giờ không , nếu có thì huỷ nó đi, tức là khi chưa hết thời gian 10s giảm tốc mà ăn thêm vật phẩm thì sẽ reset lại thời gian 10s
                    speedResetToken = new System.Threading.CancellationTokenSource();  // Sau khi hủy thì tạo cái mới để bắt đầu tính giờ lại
                    var token = speedResetToken.Token; // Lấy token từ CancellationTokenSource để truyền vào task

                    // Giảm tốc ngay
                    trafficSpeed = Math.Max(trafficSpeed - 10, 5); // Tốc độ giảm đi 10, nhưng không thấp hơn 5
                    roadSpeed = Math.Max(roadSpeed - 10, 5);
                    System.Media.SystemSounds.Hand.Play();

                    // Tạo task để chờ 10 giây rồi tăng tốc trở lại
                    Task.Delay(10000, token).ContinueWith(_ =>
                    {
                        // Nếu đã ăn vật phẩm slow rồi ăn tiếp thì token cũ bị hủy thế nên không thực hiện quay về tốc độ ban đầu
                        // Nếu đã ăn vật phẩm mà trong 10s không ăn thêm thì tức là không có lệnh hủy thế nên thực hiện câu lệnh if là quay về tốc độ ban đầu
                        if (!token.IsCancellationRequested)
                        {
                            this.Invoke((Action)(() =>
                            {
                                if (currentLevel == 1)
                                {
                                    roadSpeed = 12;
                                    trafficSpeed = 15;
                                }
                                else if (currentLevel == 2)
                                {
                                    roadSpeed = 20;
                                    trafficSpeed = 22;
                                }
                                else if (currentLevel == 3)
                                {
                                    roadSpeed = 25;
                                    trafficSpeed = 27;
                                }
                            }));
                        }
                    });

                }
            }
            else // Nếu đang ở Level 1 thì ẩn vật phẩm slow
            {
                lowSpeed.Visible = false;
            }


        

        }

        private void changeAIcars(PictureBox tempCar)
        {

            carImage = rand.Next(1, 9); // Thay đổi hình ảnh: Chọn ngẫu nhiên một ảnh mới (xe cứu thương, xe tải, xe hồng...).

            switch (carImage)
            {

                case 1:
                    tempCar.Image = Properties.Resources.ambulance;
                    break;
                case 2:
                    tempCar.Image = Properties.Resources.carGreen;
                    break;
                case 3:
                    tempCar.Image = Properties.Resources.carGrey;
                    break;
                case 4:
                    tempCar.Image = Properties.Resources.carOrange;
                    break;
                case 5:
                    tempCar.Image = Properties.Resources.carPink;
                    break;
                case 6:
                    tempCar.Image = Properties.Resources.CarRed;
                    break;
                case 7:
                    tempCar.Image = Properties.Resources.carYellow;
                    break;
                case 8:
                    tempCar.Image = Properties.Resources.TruckBlue;
                    break;
                case 9:
                    tempCar.Image = Properties.Resources.TruckWhite;
                    break;
            }


            tempCar.Top = carPosition.Next(100, 400) * -1;
            // Chọn giá trị tung độ ngẫu nhiên cho xe từ -100 đến -400 (để xe xuất hiện ngẫu nhiên phía trên khung hình) 
            // Làm vậy để xe không xuất hiện cùng hàng mà xuất hiện ngẫu nhiên khác hàng 

            if ((string)tempCar.Tag == "carLeft") // Kiểm tra nếu xe thuộc làn trái
            {
                tempCar.Left = carPosition.Next(5, 200); // Thì chọn hoành độ ngẫu nhiên trong khoảng làn trái
            }
            if ((string)tempCar.Tag == "carRight") // Kiểm tra nếu xe thuộc làn phải
            {
                tempCar.Left = carPosition.Next(245, 540); // Thì chọn hoành độ ngẫu nhiên trong khoảng làn phải
            }

            // Kết thúc hàm là đã thay đổi xong hình ảnh và vị trí của xe AI (thay)
        }

        // Hàm xử lý khi thua 
        private void gameOver()
        {
            playSound(); // Phát âm thanh va chạm
            gameTimer.Stop(); // Dừng phát tín hiệu gọi hàm gameTimerEvent
            explosion.Visible = true; // Hiện ứng nổ
            player.Controls.Add(explosion); // Thêm hiệu ứng nổ vào trong bức ảnh xe người chơi
            explosion.Location = new Point(-8, 5); // Cập nhật vị trí hiệu ứng nổ (cho nó nằm chính giữa xe)
            explosion.BackColor = Color.Transparent; // Đặt nền hiệu ứng nổ trong suốt để nhìn đẹp hơn


            btnStart.Enabled = true; // Cho phép bấm nút Start lại
            SaveToLeaderboard(playerName, score); // Lưu điểm vào bảng xếp hạng
            SaveHighScore(playerName, score); // Lưu điểm cao nếu có


            txtScore.Text = $"Điểm: {score} ";

            lblStt.Text = "          GAME OVER!";
            lblStt.Visible = true;

        }

        private System.Media.SoundPlayer bgmPlayer;


        // Thay đổi ảnh của xe khi nâng level
        private void ChangeCarImage(int level)
        {
            switch (level)
            {
                case 1:
                    player.Image = Properties.Resources.carYellow; // ảnh gốc
                    break;
                case 2:
                    player.Image = Properties.Resources.carYellowbluefire; // hoặc carYellow__2_
                    break;
                case 3:
                    player.Image = Properties.Resources.carYellowfire;
                    break;
                default:
                    player.Image = Properties.Resources.carYellowfire; // Giữ nguyên từ level 3 trở đi
                    break;
            }
        }

        //Hàm khởi động game được gọi khi mới chạy game và khi thua, tức là hàm được gọi sau khi nhấn nút Start
        private void ResetGame()
        {
            // "Dọn dẹp" bất kỳ đồng hồ đếm ngược 10 giây (của vật phẩm lowSpeed) nào có thể còn sót lại từ lượt chơi trước.
            // Sau khi hủy thì tạo mới để bắt đầu lại
            speedResetToken?.Cancel();
            speedResetToken = new System.Threading.CancellationTokenSource();
           

            // Kiểm tra thấy tên trống thì yêu cầu nhập tên
            if (string.IsNullOrEmpty(playerName))
            {
                InputNameForm nameForm = new InputNameForm(); // Nếu rỗng thì tạo form nhập tên
                if (nameForm.ShowDialog() == DialogResult.OK) // Nếu bấm OK thì lưu tên vào biến playerName
                {
                    playerName = nameForm.PlayerName; 
                }
                else return; // Nếu bấm Cancel thì thoát hàm không bắt đầu game
            }

            //  Dừng mọi nhạc và hiệu ứng cũ
            btnStart.Enabled = false; // Vô hiệu hóa nút Start để không bấm được khi đang chơi
            explosion.Visible = false;
            goleft = false;
            goright = false;
            score = 0;
            currentLevel = 1;

            // Reset tốc độ cứng về mặc định (Level 1)
            roadSpeed = 12;
            trafficSpeed = 15;
            playerSpeed = 12;
            player.Left = (panel1.Width / 2) - (player.Width / 2)+5;
            player.Top = 400;


            // Reset vị trí xe AI
            AI1.Top = carPosition.Next(200, 500) * -1;
            AI1.Left = carPosition.Next(5, 200);
            AI2.Top = carPosition.Next(200, 500) * -1;
            AI2.Left = carPosition.Next(245, 422);

            // Reset vật phẩm
            changeItemPosition(coin);
            changeItemPosition(lowSpeed);
            lowSpeed.Visible = false;
            lowSpeed.Top = -100;

            // Bật nhạc
            bgmPlayer = new System.Media.SoundPlayer(Properties.Resources.cuaanlam);
            if (isMusicOn)
                bgmPlayer.PlayLooping();
            UpdateMusicButton();
            lblStt.Visible = false; // Ẩn chữ Game Over



            gameTimer.Start();
        }




        // sự khác nhau giữa hàm restartGame và ResetGame là hàm restartGame nó có các tham số lắng nghe nút Start, khi người dùng bấm vào thì gọi hàm ResetGame để khởi động lại trò chơi
        private void restartGame(object sender, EventArgs e)
        {
            ResetGame();
            txtLevel.Text = "Level 1"; // Đặt lại level về 1
            player.Image = Properties.Resources.carYellow; // Đặt lại ảnh xe người chơi về ảnh gốc

            this.ActiveControl = null; // Mới đầu vào thì người dùng phải nhấp vào nút Start để bắt đầu nhưng sau khi bấm Start thì focus sẽ chuyển về form chính để nhận phím điều khiển trái phải 
            this.Focus(); // Chuyển focus về form chính
        }


        // cho phép sang trái phải liên tục 
        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) // Khi người dùng nhấn phím trái thì 
            {
                goleft = true; // Gán cho biến goleft = true để trong hàm gameTimerEvent sẽ lắng nghe biến này và // di chuyển xe người chơi sang trái
                e.Handled = true; // Báo với hệ điều hành win là đã xử lý phím này rồi, không cần xử lý thêm, vì nếu không thì sẽ chuyển focus sang control khác (nút Start, nút Exit...)
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
                e.Handled = true; 
            }
        }

        // Dừng chuyển làn khi thả phím
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goleft = false;
                e.Handled = true; 
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = false;
                e.Handled = true; 
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        // Âm thanh khi va chạm
        private void playSound()
        {
            System.Media.SoundPlayer playCrash = new System.Media.SoundPlayer(Properties.Resources.hit);
            playCrash.Play();
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("highscore.txt"))
                LoadHighScore();

            txtScore.Text = $"Điểm: 0  ";
            txtLevel.Text = "Level 1";

            bgmPlayer = new System.Media.SoundPlayer(Properties.Resources.cuaanlam);
            UpdateMusicButton();
            if (isMusicOn)
                bgmPlayer.PlayLooping();
            lblStt.Visible = true;
            lblStt.Left = (this.ClientSize.Width - lblStt.Width) / 2;
            lblStt.Top = (this.ClientSize.Height - lblStt.Height) / 2;
        }

        // Hàm cập nhật nút nhạc để hiển thị biểu tượng âm thanh tương ứng
        private void UpdateMusicButton()
        {
            button1.Text = isMusicOn ? "🔊" : "🔇";
        }

        private void SaveHighScore(string name, int score)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "highscore.txt");

                // Đọc điểm cao hiện tại (nếu có)
                int currentHighScore = 0;
                string currentName = "";

                if (File.Exists(filePath))
                {
                    string[] data = File.ReadAllText(filePath).Split(',');
                    if (data.Length == 2)
                    {
                        currentName = data[0];
                        int.TryParse(data[1], out currentHighScore);
                    }
                }

                // Chỉ ghi nếu điểm mới cao hơn
                if (score > currentHighScore)
                {
                    highScore = score;
                    File.WriteAllText(filePath, $"{name},{score}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu điểm cao: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHighScore()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "highscore.txt");

                if (File.Exists(filePath)) // Kiểm tra nếu file tồn tại
                {
                    string[] data = File.ReadAllText(filePath).Split(','); // Đọc nội dung file và tách thành mảng dựa trên dấu phẩy, string[] data là một mang chuõi , data[0] thì lưu tên người chơi, data[1] lưu điểm số
                    if (data.Length == 2 && int.TryParse(data[1], out int score)) // Kiểm tra độ dài của mảng, và chuyển điểm sang số nguyên nếu không chuyển được thì nó tự động gán 0 vào 
                    {
                        highScore = score;
                        // Có thể hiển thị tên người giữ kỷ lục nếu muốn
                        // string recordHolder = data[0];
                    }
                }
            }
            catch
            {
                highScore = 0;
            }
        }


        // Hàm lưu điểm vào bảng xếp hạng
        private void SaveToLeaderboard(string name, int score)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) // Kiểm tra nếu tên rỗng hoặc chỉ toàn khoảng trắng
                    name = "Người chơi"; // thì đặt tên là "Người chơi"

                string filePath = Path.Combine(Application.StartupPath, "leaderboard.txt"); // Lấy đường dẫn file leaderboard.txt chuyển thành string rồi gán vào biến filePath
                List<string> lines = new List<string>(); // Tạo danh sách rỗng để lưu các dòng từ file, đây là danh sách động khác với kiểu mảng tĩnh , 

                if (File.Exists(filePath)) // Kiểm tra nếu file tồn tại
                    lines = File.ReadAllLines(filePath).ToList(); // Đọc tất cả dòng từ file và chuyển thành danh sách

                // Tìm dòng trùng tên
                int existingIndex = lines.FindIndex(line => line.StartsWith(name + ",")); 

                if (existingIndex >= 0) // Nếu tìm thấy tên trùng
                {
                    string[] parts = lines[existingIndex].Split(','); // Tách dòng thành mảng dựa trên dấu phẩy
                    if (parts.Length == 2 && int.TryParse(parts[1], out int oldScore)) // Kiểm tra độ dài mảng và chuyển điểm cũ sang số nguyên
                    {
                        if (score > oldScore) // Nếu điểm mới cao hơn điểm cũ
                        {
                            DialogResult result = MessageBox.Show(
                                $"Người chơi \"{name}\" đã có kỷ lục {oldScore} điểm.\n" +
                                $"Bạn có muốn thay bằng điểm mới {score} không?",
                                "Cập nhật kỷ lục",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (result == DialogResult.Yes) // Nếu người dùng chọn có thì thay thế
                            {
                                lines[existingIndex] = $"{name},{score}";
                            }
                            else
                            {
                                return; // không thay thế
                            }
                        }
                        else
                        {
                            // Điểm thấp hơn, không ghi đè
                            MessageBox.Show($"Bạn chưa vượt qua kỷ lục {oldScore} của mình.",
                                            "Thông báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                else
                {
                    // Nếu chưa có tên đó thì thêm mới
                    string entry = $"{name},{score}"; // Tạo dòng mới với định dạng "tên,điểm"
                    lines.Add(entry); // Thêm dòng mới vào danh sách
                }

                // Sắp xếp theo điểm từ cao xuống thấp
                var sortedLines = lines // Quét từng dòng trong danh sách lines
                    .Select(line =>
                    { // line là tên biến tạm đại diện cho từng dòng trong danh sách lines , với mỗi hàng thì tách nó thành mảng parts dựa trên dấu phẩy
                        var parts = line.Split(',');
                        return new // trả về một đối tượng mới có các thuộc tính Name, Score, Original
                        {
                            Name = parts[0], // tên một ngăn tên là Name và gán parts vào đó 
                            Score = parts.Length == 2 && int.TryParse(parts[1], out int s) ? s : 0, // Tạo một ngăn tên là Score và gán điểm số vào đó, nếu không chuyển được thì gán 0
                            Original = line // Tạo một ngăn tên là Original để lưu dòng gốc 
                        };
                    })
                    .OrderByDescending(x => x.Score) // Sắp xếp danh sách theo điểm số từ cao xuống thấp
                    .Select(x => $"{x.Name},{x.Score}") // Chuyển lại thành định dạng "tên,điểm
                    .ToList(); // Chuyển kết quả cuối cùng thành danh sách

                File.WriteAllLines(filePath, sortedLines); // Ghi tất cả dòng đã sắp xếp trở lại file
            }
            catch (Exception ex) // Bắt lỗi nếu có
            {
                MessageBox.Show("Lỗi khi lưu điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnLeaderboard_Click(object sender, EventArgs e) // Xem bảng xếp hạng
        {
            LeaderboardForm leaderboard = new LeaderboardForm(); // Tạo đối tượng form bảng xếp hạng
            leaderboard.ShowDialog(); // Hiển thị form bảng xếp hạng dưới dạng hộp thoại modal
        }

        

        private void button1_Click_1(object sender, EventArgs e) // Nút bật tắt nhạc nền
        {
            if (bgmPlayer == null) return; // nếu chưa khởi tạo nhạc nền thì bỏ qua
           
            if (isMusicOn)
            {
                bgmPlayer.Stop(); // Nếu nhạc đang bật thì tắt
                button1.Text = "🔇";
                isMusicOn = false;  
            }
            else
            {
                bgmPlayer.PlayLooping(); // Nếu nhạc đang tắt thì bật
                button1.Text = "🔊";
                isMusicOn = true;
            }

        }

        // Quá tải phương thức xử lý phím để nhận phím trái phải dù đang focus ở bất kỳ control nào
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left) // Nếu phím trái được nhấn
            {
                goleft = true;
                return true; // Thông báo với win là đã xử lý phím này rồi, không gửi phím này đến control khác
            }
            else if (keyData == Keys.Right)
            {
                goright = true;
                return true;
            }
            

            return base.ProcessCmdKey(ref msg, keyData); // Nếu phím bấm không phải trái phải thì gọi phương thức gốc của win xử lý bình thường
        }

        private void btnExit_Click(object sender, EventArgs e) // Nút thoát game
        {
            Application.Exit(); // Thoát ứng dụng
        }

        // Hàm thay đổi vị trí vật phẩm (coin, lowSpeed)
        private void changeItemPosition(PictureBox item)
        {
            item.Top = carPosition.Next(100, 400) * -1; // Chọn tung độ ngẫu nhiên phía trên khung hình

            if ((string)item.Tag == "carLeft")
            {
                item.Left = carPosition.Next(5, 200);
            }
            else if ((string)item.Tag == "carRight")
            {
                item.Left = carPosition.Next(245, 422);
            }
            else
            {
                // coin hoặc lowSpeed không có tag cụ thể → đặt ngẫu nhiên trong 2 làn
                item.Left = carPosition.Next(5, 422);
            }
        }

    }

    }

