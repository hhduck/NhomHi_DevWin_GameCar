using System;
using System.IO;
using System.Windows.Forms;

namespace Car_Racing_Game_MOO_ICT
{
    public partial class LeaderboardForm : Form
    {
        public LeaderboardForm()
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            string filePath = Path.Combine(Application.StartupPath, "leaderboard.txt");
            listView1.Items.Clear();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        var item = new ListViewItem(parts[0]); // Tên người chơi gán vào cột đầu tiên
                        item.SubItems.Add(parts[1]); // Điểm số gán vào cột thứ hai
                        listView1.Items.Add(item); // Thêm mục vào ListView
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa toàn bộ bảng xếp hạng?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string leaderboardPath = Path.Combine(Application.StartupPath, "leaderboard.txt");
                    string highscorePath = Path.Combine(Application.StartupPath, "highscore.txt");

                    // Xóa file leaderboard
                    if (File.Exists(leaderboardPath))
                        File.Delete(leaderboardPath);

                    // Xóa file highscore
                    if (File.Exists(highscorePath))
                        File.Delete(highscorePath);

                    MessageBox.Show("Đã xóa bảng xếp hạng và điểm cao nhất!",
                                  "Thành công",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);

                    // Refresh lại danh sách hiển thị (nếu có)
                    LoadLeaderboard(); // hoặc đóng form
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message,
                                  "Lỗi",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }
    }
}
