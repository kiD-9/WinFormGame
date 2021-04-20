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
using GoldenCity.Models;

namespace GoldenCity.Forms
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private int bitmapSize = 175;
        private int mapSize = 5;
        private GameSetting gameSetting;
        
        public Form1() //TODO я только попробовал поотрисовывать элементы карты
        {
            DoubleBuffered = true;
            InitializeComponent();
            
            
            gameSetting = new GameSetting(mapSize, 10000, true);
            gameSetting.AddBuilding(new SheriffsHouse(3, 2)); //почему он не добавляется
            gameSetting.AddBuilding(new Store(1, 0));
            gameSetting.AddBuilding(new LivingHouse(2,0));
            ClientSize = new Size(mapSize * bitmapSize, mapSize * bitmapSize + 30);
            
            var imagesDirectory = new DirectoryInfo("Resources");
            foreach (var fileInfo in imagesDirectory.GetFiles("*.png"))
            {
                bitmaps[fileInfo.Name] = (Bitmap) Image.FromFile(fileInfo.FullName);
            }

            AddControls();
            
            var repaintTimer = new Timer {Interval = 15};
            repaintTimer.Tick += TimerTick;
            repaintTimer.Start();
            //
            // Click += HandleClick;
            // DoubleClick += HandleClick;
        }

        protected  override void OnPaint(PaintEventArgs e)
        {
            UpdateGamePropertiesPaint(); //TODO переделать костыль
            DrawBackground(e.Graphics);
            DrawBuildings(e.Graphics);
        }

        private void HandleClick(object sender, EventArgs e) //TODO
        {
            var clickArgs = e as MouseEventArgs;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            gameSetting.ChangeMoney(100);// для проверки отрисовки добавил доход
            Invalidate(); //но значение почему-то не меняется и не рисуется
        }

        private void DrawBackground(Graphics graphics)
        {
            for (var i = 0; i < mapSize * mapSize; i++)
            {
                var point = new Point(bitmapSize * (i % mapSize), bitmapSize * (i / mapSize) + 30);
                graphics.DrawImage(bitmaps["ground.png"], point);
                graphics.DrawRectangle(new Pen(Color.Sienna, 8), new Rectangle(point, new Size(bitmapSize, bitmapSize)));
            }
        }

        private void DrawBuildings(Graphics graphics)
        {
            var counter = 0;
            foreach (var building in gameSetting.Map)
            {
                var point = new Point(bitmapSize * (counter % mapSize), bitmapSize * (counter / mapSize) + 30);
                switch (building)
                {
                    case Jail:
                        graphics.DrawImage(bitmaps["Jail.png"], point);
                        break;
                    case LivingHouse:
                        graphics.DrawImage(bitmaps["LivingHouse.png"], point);
                        break;
                    case RailroadStation:
                        graphics.DrawImage(bitmaps["RailroadStation.png"], point);
                        break;
                    case Saloon:
                        graphics.DrawImage(bitmaps["Saloon.png"], point);
                        break;
                    case SheriffsHouse:
                        graphics.DrawImage(bitmaps["SheriffsHouse.png"], point);
                        break;
                    case Store:
                        graphics.DrawImage(bitmaps["Store.png"], point);
                        break;
                }
                counter++;
            }
        }

        private void AddControls()
        {
            var money = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width / 5, 26),
                Text = $"Money: {gameSetting.Money}",
            };
            //money.TextChanged += MoneyTextChanged; //не совсем понимаю как текст обновлять

            var citiziensCount = new Label
            {
                Location = new Point(ClientSize.Width / 5, 0),
                Size = new Size(ClientSize.Width / 5, 26),
                Text = $"Citiziens: {gameSetting.citiziens.Count}"
            };
            //citiziensCount.TextChanged += CitiziensCountTextChanged;

            var citiziensLimit = new Label
            {
                Location = new Point(2 * ClientSize.Width / 5, 0),
                Size = new Size(ClientSize.Width / 5, 26),
                Text = $"Citiziens Limit: {gameSetting.CitiziensLimit}"
            };
            //citiziensLimit.TextChanged += CitiziensLimitTextChanged;

            var sheriffsCount = new Label
            {
                Location = new Point(3 * ClientSize.Width / 5, 0),
                Size = new Size(ClientSize.Width / 5, 26),
                Text = $"Sheriffs: {gameSetting.SheriffsCount}",
            };
            //sheriffsCount.TextChanged += SheriffsCountTextChanged;
            
            Controls.Add(money);
            Controls.Add(citiziensCount);
            Controls.Add(citiziensLimit);
            Controls.Add(sheriffsCount);
        }

        private void UpdateGamePropertiesPaint()
        {
            Controls[0].Text = $"Money: {gameSetting.Money}";
            Controls[1].Text = $"Citiziens: {gameSetting.citiziens.Count}";
            Controls[2].Text = $"Citiziens Limit: {gameSetting.CitiziensLimit}";
            Controls[3].Text = $"Sheriffs: {gameSetting.SheriffsCount}";
        }
        // private void MoneyTextChanged(object sender, EventArgs e)
        // {
        //     Controls[0].Text = $"Money: {gameSetting.Money}";
        // }
        //
        // private void CitiziensCountTextChanged(object sender, EventArgs e)
        // {
        //     Controls[1].Text = $"Citiziens: {gameSetting.citiziens.Count}";
        // }
        //
        // private void CitiziensLimitTextChanged(object sender, EventArgs e)
        // {
        //     Controls[2].Text = $"Citiziens Limit: {gameSetting.CitiziensLimit}";
        // }
        //
        // private void SheriffsCountTextChanged(object sender, EventArgs e)
        // {
        //     Controls[3].Text = $"Sheriffs: {gameSetting.SheriffsCount}";
        // }
    }
}