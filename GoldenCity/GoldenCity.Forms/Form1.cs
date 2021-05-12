using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GoldenCity.Models;
using Timer = System.Windows.Forms.Timer;

namespace GoldenCity.Forms
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private const int GamePropertiesBarHeight = 40;
        private const int BorderLineWidth = 8;
        private const int BitmapSize = 120;
        private const int MapSize = 5;
        private readonly GameSetting gameSetting;
        
        public Form1()
        {
            DoubleBuffered = true;
            InitializeComponent();
            
            gameSetting = new GameSetting(MapSize, 15000);
            ClientSize = new Size(MapSize * BitmapSize, MapSize * BitmapSize + GamePropertiesBarHeight);
            for (var i = 0; i < 4; i++)
            {
                gameSetting.AddCitizen();
            }

            TakeBitmapsFromDirectory(new DirectoryInfo("Resources"));
            
            AddPropertiesControls();
            
            var repaintTimer = new Timer {Interval = 15};
            repaintTimer.Tick += TimerTick;
            repaintTimer.Start();
            
            InitializeGameTimers();
            
            Click += HandleClick;
        }
        
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateGamePropertiesValues();
            DrawBackground(e.Graphics);
            DrawBuildings(e.Graphics);
        }

        private void HandleClick(object sender, EventArgs e) //TODO поправить в соответствии с возможностями игрока относительно текущей игровой ситуации
        {
            var clickArgs = e as MouseEventArgs;
            if (clickArgs == null || clickArgs.Y <= GamePropertiesBarHeight)
                return;

            var buildingLocation = new Point(clickArgs.X / BitmapSize,
                (clickArgs.Y - GamePropertiesBarHeight) / BitmapSize);

            var contextMenuStrip = new ContextMenuStrip
            {
                BackColor = Color.Chocolate
            };

            if (gameSetting.Map[buildingLocation.Y, buildingLocation.X] == null)
                contextMenuStrip.Items.Add(CreateAddBuildingMenuItem(buildingLocation));
            else
            {
                var toolStripMenuItemDeleteBuilding = new ToolStripMenuItem("Delete building", null,
                    (s, args) =>
                        gameSetting.DeleteBuilding(buildingLocation.X, buildingLocation.Y))
                {
                    BackColor = Color.Chocolate
                };

                var toolStripMenuItemAddWorker = new ToolStripMenuItem("Add worker", null,
                    (s, args) =>
                        gameSetting.AddWorker(gameSetting.Map[buildingLocation.Y, buildingLocation.X]))
                {
                    BackColor = Color.Chocolate
                };

                var toolStripMenuItemRetireWorker = new ToolStripMenuItem("Retire worker", null,
                    (s, args) =>
                        gameSetting.RetireWorker(gameSetting.Map[buildingLocation.Y, buildingLocation.X].WorkerId))
                {
                    BackColor = Color.Chocolate
                };

                contextMenuStrip.Items.AddRange(new ToolStripItem[]
                    {toolStripMenuItemDeleteBuilding, toolStripMenuItemAddWorker, toolStripMenuItemRetireWorker});
            }

            contextMenuStrip.Show(clickArgs.Location);
        }

        private ToolStripMenuItem CreateAddBuildingMenuItem(Point buildingLocation)
        {
            var toolStripMenuItemJail = new ToolStripMenuItem("Jail - 4000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Jail(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemLivingHouse = new ToolStripMenuItem("Living house - 500 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new LivingHouse(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemRailroadStation = new ToolStripMenuItem("Railroad station - 3000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new RailroadStation(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSaloon = new ToolStripMenuItem("Saloon - 1500 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Saloon(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSheriffsHouse = new ToolStripMenuItem("Sheriffs house - 5000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new SheriffsHouse(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemStore = new ToolStripMenuItem("Store - 3000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Store(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };

            return new ToolStripMenuItem("Add building")
            {
                DropDownItems =
                {
                    toolStripMenuItemJail, toolStripMenuItemLivingHouse,
                    toolStripMenuItemRailroadStation, toolStripMenuItemSaloon,
                    toolStripMenuItemSheriffsHouse, toolStripMenuItemStore
                },
                BackColor = Color.Chocolate
            };
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // gameSetting.ChangeMoney(100);// для проверки отрисовки добавил доход
            Invalidate();
        }

        private void DrawBackground(Graphics graphics)
        {
            for (var i = 0; i < MapSize * MapSize; i++)
            {
                var point = new Point(BitmapSize * (i % MapSize), BitmapSize * (i / MapSize) + GamePropertiesBarHeight);
                graphics.DrawImage(bitmaps["Background.png"], point);
                graphics.DrawRectangle(new Pen(Color.Sienna, BorderLineWidth), new Rectangle(point, new Size(BitmapSize, BitmapSize)));
            }
        }

        private void DrawBuildings(Graphics graphics)
        {
            var counter = 0;
            foreach (var building in gameSetting.Map)
            {
                var point = new Point(BitmapSize * (counter % MapSize), BitmapSize * (counter / MapSize) + GamePropertiesBarHeight);
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

        private void AddPropertiesControls()
        {
            var moneyLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Money: {gameSetting.Money} $",
                BackColor = Color.Chocolate
            };

            var citiziensCountLabel = new Label
            {
                Location = new Point(ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Citiziens: {gameSetting.citizens.Count}",
                BackColor = Color.Chocolate
            };

            var citiziensLimitLabel = new Label
            {
                Location = new Point(2 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Citiziens Limit: {gameSetting.CitizensLimit}",
                BackColor = Color.Chocolate
            };

            var sheriffsCountLabel = new Label
            {
                Location = new Point(3 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Sheriffs: {gameSetting.SheriffsCount}",
                BackColor = Color.Chocolate
            };

            var attackTimerLabel = new Label
            {
                Location = new Point(4 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Attack timer: {gameSetting.AttackTimerInterval / 1000} sec", // можно попробовать сделать динамически меняющимся
                BackColor = Color.Chocolate
            };
            
            Controls.Add(moneyLabel);
            Controls.Add(citiziensCountLabel);
            Controls.Add(citiziensLimitLabel);
            Controls.Add(sheriffsCountLabel);
            Controls.Add(attackTimerLabel);
        }

        private void UpdateGamePropertiesValues()
        {
            Controls[0].Text = $"Money: {gameSetting.Money} $";
            Controls[1].Text = $"Citiziens: {gameSetting.citizens.Count}";
            Controls[2].Text = $"Citiziens Limit: {gameSetting.CitizensLimit}";
            Controls[3].Text = $"Sheriffs: {gameSetting.SheriffsCount}";
            Controls[4].Text = $"Attack timer: {gameSetting.AttackTimerInterval / 1000} sec";
        }

        private void TakeBitmapsFromDirectory(DirectoryInfo imagesDirectoryInfo)
        {
            foreach (var fileInfo in imagesDirectoryInfo.GetFiles("*.png"))
            {
                bitmaps[fileInfo.Name] = (Bitmap) Image.FromFile(fileInfo.FullName);
            }
        }
    }
}