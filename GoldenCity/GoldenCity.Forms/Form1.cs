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
            
            gameSetting = new GameSetting(MapSize, 10000, true);
            ClientSize = new Size(MapSize * BitmapSize, MapSize * BitmapSize + GamePropertiesBarHeight);
            for (var i = 0; i < 4; i++)
            {
                gameSetting.AddCitizien(null);
            }

            TakeBitmapsFromDirectory(new DirectoryInfo("Resources"));
            
            AddPropertiesControls();
            
            var repaintTimer = new Timer {Interval = 15};
            repaintTimer.Tick += TimerTick;
            repaintTimer.Start();
            
            //TODO игровые таймеры
            
            Click += HandleClick;
        }

        protected  override void OnPaint(PaintEventArgs e)
        {
            DrawBackground(e.Graphics);
            DrawBuildings(e.Graphics);
            UpdateGamePropertiesValues();
        }

        private void HandleClick(object sender, EventArgs e) //TODO
        {
            var clickArgs = e as MouseEventArgs;
            if (clickArgs.Y <= GamePropertiesBarHeight)
                return;

            var buildingLocation = new Point(clickArgs.X / BitmapSize,
                (clickArgs.Y - GamePropertiesBarHeight) / BitmapSize);
            
            var toolStripMenuItemJail = new ToolStripMenuItem("Jail", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Jail(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemLivingHouse = new ToolStripMenuItem("Living house", null,
                (o, args) =>
                    gameSetting.AddBuilding(new LivingHouse(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemRailroadStation = new ToolStripMenuItem("Railroad station", null,
                (o, args) =>
                    gameSetting.AddBuilding(new RailroadStation(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSaloon = new ToolStripMenuItem("Saloon", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Saloon(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSheriffsHouse = new ToolStripMenuItem("Sheriffs house", null,
                (o, args) =>
                    gameSetting.AddBuilding(new SheriffsHouse(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemStore = new ToolStripMenuItem("Store", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Store(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };

            var toolStripMenuItemAddBuilding = new ToolStripMenuItem("Add building")
            {
                DropDownItems =
                {
                    toolStripMenuItemJail, toolStripMenuItemLivingHouse,
                    toolStripMenuItemRailroadStation, toolStripMenuItemSaloon,
                    toolStripMenuItemSheriffsHouse, toolStripMenuItemStore
                },
                BackColor = Color.Chocolate
            };
            
            
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


            var contextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    toolStripMenuItemAddBuilding, toolStripMenuItemDeleteBuilding,
                    toolStripMenuItemAddWorker, toolStripMenuItemRetireWorker
                },
                BackColor = Color.Chocolate
            };
            contextMenuStrip.Show(clickArgs.Location);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            gameSetting.ChangeMoney(100);// для проверки отрисовки добавил доход
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
            var money = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Money: {gameSetting.Money}",
                BackColor = Color.Chocolate
            };

            var citiziensCount = new Label
            {
                Location = new Point(ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Citiziens: {gameSetting.citiziens.Count}",
                BackColor = Color.Chocolate
            };

            var citiziensLimit = new Label
            {
                Location = new Point(2 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Citiziens Limit: {gameSetting.CitiziensLimit}",
                BackColor = Color.Chocolate
            };

            var sheriffsCount = new Label
            {
                Location = new Point(3 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Sheriffs: {gameSetting.SheriffsCount}",
                BackColor = Color.Chocolate
            };

            var attackTimer = new Label
            {
                Location = new Point(4 * ClientSize.Width / MapSize, 0),
                Size = new Size(ClientSize.Width / MapSize, GamePropertiesBarHeight),
                Text = $"Attack timer: {gameSetting.AttackTimerInterval / 1000} sec", // можно попробовать сделать динамически меняющимся
                BackColor = Color.Chocolate
            };
            
            Controls.Add(money);
            Controls.Add(citiziensCount);
            Controls.Add(citiziensLimit);
            Controls.Add(sheriffsCount);
            Controls.Add(attackTimer);
        }

        private void UpdateGamePropertiesValues()
        {
            Controls[0].Text = $"Money: {gameSetting.Money}";
            Controls[1].Text = $"Citiziens: {gameSetting.citiziens.Count}";
            Controls[2].Text = $"Citiziens Limit: {gameSetting.CitiziensLimit}";
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