using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GoldenCity.Models;

namespace GoldenCity.Forms
{
    public partial class GameControl : UserControl
    {
        public const int BitmapSize = 120;
        public const int GamePropertiesBarHeight = 40;
        private readonly MainForm mainForm;
        private const int BorderLineWidth = 8;
        private const int MapSize = 5;
        private GameSetting gameSetting;
        
        public GameControl(MainForm mainForm)
        {
            DoubleBuffered = true;
            InitializeComponent();
            this.mainForm = mainForm;
            ClientSize = mainForm.ClientSize;
            
            gameSetting = new GameSetting(MapSize, 15000);
            AddPropertiesControls();
            Click += HandleClick;
        }

        public void StartGame()
        {
            gameSetting = new GameSetting(MapSize, 15000);
            InitializeTimers();
        }

        private void HandleClick(object sender, EventArgs e)
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
                contextMenuStrip.Items.Add(toolStripMenuItemDeleteBuilding);

                if (gameSetting.Map[buildingLocation.Y, buildingLocation.X].WorkerId < 0)
                {
                    contextMenuStrip.Items.Add(
                        new ToolStripMenuItem("Add worker", null,
                            (s, args) =>
                                gameSetting.AddWorker(gameSetting.Map[buildingLocation.Y, buildingLocation.X]))
                        {
                            BackColor = Color.Chocolate
                        }
                    );
                }
                else
                {
                    contextMenuStrip.Items.Add(
                        new ToolStripMenuItem("Retire worker", null,
                            (s, args) =>
                                gameSetting.RetireWorker(gameSetting.Map[buildingLocation.Y, buildingLocation.X]
                                    .WorkerId))
                        {
                            BackColor = Color.Chocolate
                        }
                    );
                }
            }

            contextMenuStrip.Show(clickArgs.Location);
        }

        private ToolStripMenuItem CreateAddBuildingMenuItem(Point buildingLocation)
        {
            var toolStripMenuItemJail = new ToolStripMenuItem("Jail - 4.000 $", null,
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
            var toolStripMenuItemRailroadStation = new ToolStripMenuItem("Railroad station - 3.000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new RailroadStation(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSaloon = new ToolStripMenuItem("Saloon - 1.500 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Saloon(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemSheriffsHouse = new ToolStripMenuItem("Sheriffs house - 5.000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new SheriffsHouse(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemStore = new ToolStripMenuItem("Store - 3.000 $", null,
                (o, args) =>
                    gameSetting.AddBuilding(new Store(buildingLocation.X, buildingLocation.Y)))
            {
                BackColor = Color.Chocolate
            };
            var toolStripMenuItemTownHall = new ToolStripMenuItem("Town hall - 100.000 $", null,
                (o, args) =>
                {
                    gameSetting.AddBuilding(new TownHall(buildingLocation.X, buildingLocation.Y));
                    CheckIfGameFinished();
                })
            {
                BackColor = Color.Chocolate
            };

            return new ToolStripMenuItem("Add building")
            {
                DropDownItems =
                {
                    toolStripMenuItemJail, toolStripMenuItemLivingHouse,
                    toolStripMenuItemRailroadStation, toolStripMenuItemSaloon,
                    toolStripMenuItemSheriffsHouse, toolStripMenuItemStore,
                    toolStripMenuItemTownHall
                },
                BackColor = Color.Chocolate
            };
        }

        private void CheckIfGameFinished()
        {
            if (gameSetting.IsGameFinished)
            {
                var timer = new Timer{Interval = 2000};
                timer.Tick += (s, e) =>
                {
                    mainForm.ShowFinishedControl();
                    StopTimers();
                    timer.Stop();
                };
                timer.Start();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateGamePropertiesValues();
            DrawBackground(e.Graphics);
            DrawBuildings(e.Graphics);
            if (banditsDrawingTimerInterval > 0)
            {
                banditsDrawingTimerInterval -= 100;
                DrawBandits(e.Graphics);
            }
        }

        private void DrawBackground(Graphics graphics)
        {
            for (var i = 0; i < MapSize * MapSize; i++)
            {
                var point = new Point(BitmapSize * (i % MapSize), BitmapSize * (i / MapSize) + GamePropertiesBarHeight);
                graphics.DrawImage(mainForm.Bitmaps["Background.png"], point);
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
                        graphics.DrawImage(mainForm.Bitmaps["Jail.png"], point);
                        break;
                    case LivingHouse:
                        graphics.DrawImage(mainForm.Bitmaps["LivingHouse.png"], point);
                        break;
                    case RailroadStation:
                        graphics.DrawImage(mainForm.Bitmaps["RailroadStation.png"], point);
                        break;
                    case Saloon:
                        graphics.DrawImage(mainForm.Bitmaps["Saloon.png"], point);
                        break;
                    case SheriffsHouse:
                        graphics.DrawImage(mainForm.Bitmaps["SheriffsHouse.png"], point);
                        break;
                    case Store:
                        graphics.DrawImage(mainForm.Bitmaps["Store.png"], point);
                        break;
                    case TownHall:
                        graphics.DrawImage(mainForm.Bitmaps["TownHall.png"], point);
                        break;
                }
                counter++;
            }
        }
        
        private void DrawBandits(Graphics graphics) //todo todo todo todo todo todo 
        {
            foreach (var building in gameSetting.BuildingsToRaid.Where(b => b != null))
            {
                var point = new Point(building.X * BitmapSize, building.Y * BitmapSize + GamePropertiesBarHeight);
                graphics.DrawImage(mainForm.Bitmaps["Bandit.png"], point);
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
                Text = $"Citiziens: {gameSetting.CitizensCount}",
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
            Controls[1].Text = $"Citiziens: {gameSetting.CitizensCount}";
            Controls[2].Text = $"Citiziens Limit: {gameSetting.CitizensLimit}";
            Controls[3].Text = $"Sheriffs: {gameSetting.SheriffsCount}";
            Controls[4].Text = $"Attack timer: {gameSetting.AttackTimerInterval / 1000} sec";
        }
    }
}