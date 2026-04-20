using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfEscapeGame.Classes;

namespace WpfEscapeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Room currentRoom;
        public MainWindow()
        {
            InitializeComponent();

            // define room
            Room room1 = new Room()
            {
                Name = "bedroom",
                Description = "I seem to be in a medium sized bedroom.There is a locker to the left, a nice rug on the floor, and a bed to the right. ",
                Image = "Images/ss-bedroom.png"
            };
            Room room2 = new Room()
            {
                Name = "living room",
                Description = "I seem to be in the living room. Very intresting.",
                Image = "Images/ss-living.png"
            };
    
            Room room3 = new Room()
            {
                Name = "computer room",
                Description = "I seem to be in the computer room. Let's look around.",
                Image = "Images/ss-computer.png"
            };

            // define items
            Item key1 = new Item()
            {
                Name = "small silver key",
                Description = "A small silver key, makes me think of one I had at highschool."
            };

            Item key2 = new Item()
            {
                Name = "large key",
                Description = "A large key. Could this be my way out?"
            };
            Item locker = new Item()
            {
                Name = "locker",
                Description = "A locker. I wonder what's inside."
            };
            locker.IsPortable = false;
            locker.HiddenItem = key2;
            locker.IsLocked = true;
            locker.Key = key1;
            Item bed = new Item()
            {
                Name = "bed",
                Description = "Just a bed. I am not tired now."
            };
            bed.IsPortable = false;
            bed.HiddenItem = key1;
            Item chair = new Item()
            {
                Name = "chair",
                Description = "A chair. Weird to have that here"
            };
            chair.IsPortable = false;
            Item poster = new Item()
            {
                Name = "poster",
                Description = "Intresting wall decoration"
            };

            Item television = new Item()
            {
                Name = "television",
                Description = "This must've been expensive"
            };
            television.IsPortable = false;

            Item bin = new Item()
            {
                Name = "bin",
                Description = "Trash recycling is very important"
            };
            bin.IsPortable = true;

            Item computer = new Item()
            {
                Name = "Computer",
                Description = "This is a computer. It looks so old"
            };
            computer.IsPortable = true;

            Item portrait = new Item()
            {
                Name = "portrait",
                Description = "This is a portrait. I wonder who that might be."
            };

            Item couch = new Item()
            {
                Name = "couch",
                Description = "This is a couch. It looks comfy."
            };

            Item cupboard = new Item()
            {
                Name = "cupboard",
                Description = "This is a cupboard. It looks ancient, its probably expensive."
            };

            Item plant = new Item()
            {
                Name = "plant",
                Description = "This is a plant. Somebody has watered it."
            };

            // Doors
            Doors door1 = new Doors()
            {
                Name = "green door",
                Description = "What a bright coloured door. Lets look what's behind it."
            };
            door1.ToRoom = room2;
            door1.IsLocked = true;
            door1.Key = key2;

            Doors door2 = new Doors()
            {
                Name = "white door left",
                Description = "A white door. What a bland color for a door."
            };
            door2.ToRoom = room3;
            Doors door3 = new Doors()
            {
                Name = "computer room door",
                Description = "Atleast this door is open."
            };
            door3.ToRoom = room2;
            Doors door4 = new Doors()
            {
                Name = "closed living room door",
                Description = "I don't think I'll be able to open this one."
            };
            door4.ToRoom = null;
            

            // setup bedroom
            room1.Items.Add(new Item()
            {
                Name = "floor mat",
                Description = "A bit ragged floor mat, but still one of the most popular designs. "
            });
            room1.Items.Add(bed);
            room1.Items.Add(locker);
            room1.Doors.Add(door1);


            room2.Items.Add(bin);
            room2.Items.Add(computer);
            room2.Items.Add(portrait);
            room2.Doors.Add(door2);
            room2.Doors.Add(door4);

            room3.Items.Add(couch);
            room3.Items.Add(plant);
            room3.Items.Add(cupboard);
            room3.Doors.Add(door3);


            // start game
            currentRoom = room1;
            txtMessage.Text = "I am awake, but cannot remember who I am! ? Must have been a hell of a party last night... ";
            txtRoomDesc.Text = currentRoom.Description;
            UpdateUI();
        }

        private void UpdateUI()
        {
            lstRoomItems.Items.Clear();
            foreach (Item itm in currentRoom.Items)
            {
                lstRoomItems.Items.Add(itm);
            }

            lstRoomDoors.Items.Clear();
            foreach (Doors door in currentRoom.Doors)
            {
                lstRoomDoors.Items.Add(door);
            }
        }




        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnCheck.IsEnabled = lstRoomItems.SelectedValue != null; // room item selected
            btnPickUp.IsEnabled = lstRoomItems.SelectedValue != null; // room item selected
            btnUseOn.IsEnabled = lstRoomItems.SelectedValue != null && lstMyItems.SelectedValue != null; // room item and picked up item selected
            btnDrop.IsEnabled = lstMyItems.SelectedValue != null;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            // 1. find item to check
            Item roomItem = (Item)lstRoomItems.SelectedItem;
            // 2. is it locked?
            if (roomItem.IsLocked)
            {
                txtMessage.Text = $"{roomItem.Description}. It is firmly locked. ";
                return;
            }
            // 3. does it contain a hidden item?
            Item foundItem = roomItem.HiddenItem;
            if (foundItem != null)
            {
                txtMessage.Text = $"Oh, look, I found a {foundItem.Name}. ";
                lstMyItems.Items.Add(foundItem);
                roomItem.HiddenItem = null;
                return;
            }
            // 4. just another item; show description
            txtMessage.Text = roomItem.Description;
        }

        private void BtnUseOn_Click(object sender, RoutedEventArgs e)
        {
            // 1. find both items
            Item myItem = (Item)lstMyItems.SelectedItem;
            Item roomItem = (Item)lstRoomItems.SelectedItem;
            // 2. item doesn't fit
            if (roomItem.Key != myItem)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(Enums.MessageType.PastNiet);
                return;
            }
            // 3. item fits; other item unlocked
            roomItem.IsLocked = false;
            roomItem.Key = null;
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"I just unlocked the {roomItem.Name}!";
        }

        private void BtnPickUp_Click(object sender, RoutedEventArgs e)
        {
            // 1. find selected item
            Item selItem = (Item)lstRoomItems.SelectedItem;
            // 2. add item to your items list
            txtMessage.Text = $"I just picked up the {selItem.Name}. ";
            lstMyItems.Items.Add(selItem);
            lstRoomItems.Items.Remove(selItem);
            currentRoom.Items.Remove(selItem);
        }

        private void BtnDrop_Click(object sender, RoutedEventArgs e)
        {
            // 1. find selected item
            Item selItem = (Item)lstRoomItems.SelectedItem;
            // 2. add item to your items list
            txtMessage.Text = $"I just dropped the {selItem.Name}. ";
            lstMyItems.Items.Remove(selItem);
            lstRoomItems.Items.Add(selItem);
            currentRoom.Items.Add(selItem);
        }

        private void LstDoors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool Doors = lstRoomDoors.SelectedItem != null;
            btnEnter.IsEnabled = Doors;
            btnOpenWith.IsEnabled = Doors && lstMyItems.SelectedItem != null;
            btnUseOn.IsEnabled = Doors && lstMyItems.SelectedItem != null;
        }

        private void BtnOpenWith_Click(object sender, RoutedEventArgs e)
        {
            Doors myDoor = (Doors)lstRoomDoors.SelectedItem;
            Item myKey = (Item)lstMyItems.SelectedItem;

            if (myDoor.Key != myKey)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(Enums.MessageType.PastNiet);
                return;
            }

            myDoor.IsLocked = false;
            myDoor.Key = null;
            lstMyItems.Items.Remove(myKey);
            txtMessage.Text = $"I just unlocked the {myDoor.Name}!";

            UpdateUI();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            Doors myDoor = (Doors)(lstRoomDoors.SelectedItem);

            if (myDoor.IsLocked)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(Enums.MessageType.KanNiet);
                return;
            }

            currentRoom = myDoor.ToRoom;

            txtRoomDesc.Text = currentRoom.Description;

            UpdateUI();
        }
    }
}