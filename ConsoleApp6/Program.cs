using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    class Program
    {
        static Pickaxe pickaxe;
        static Player p;
        static Random rand;
        const int INVALID_INDEX = -1;
        static List<Cave> caves;
        static int currentCave = INVALID_INDEX;
        static bool isBase = false;
        static bool isDead = false;
        const int WIN_DIAMONDS = 3;
        static bool goToCave = true;

        static void Main(string[] args)
        {
            Initialize();

            Console.WriteLine("Добро пожаловать! Ваша задача - найти 3 алмаза и остаться живым!");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - -");
            ReturnToBase(false);

            bool isGame = true;
            while(isGame)
            {
                if(isBase)
                {
                    ReturnToBase(true);
                    isBase = false;
                }

                if(currentCave != INVALID_INDEX)
                {                    
                    GoToCave(caves[currentCave]);
                }

                if (p.Diamonds == WIN_DIAMONDS)
                {
                    Win();
                    Console.ReadKey();
                    return;                    
                }

                if (isDead)
                {
                    Lose();
                    return;
                }
            }

            Console.ReadKey();
        }
       
        static void Win()
        {
            Console.WriteLine("Ты победил!");
        }

        static void Lose()
        {
            Console.WriteLine("Ты умер!");
        }

        static void ReturnToBase(bool isReturned)
        {
            bool changeInventory = AskYesNo($"Вы {(isReturned ? "пришли на базу" : "находитесь на базе")}. Желаете поменять инвентарь?");
            PrintInventory();

            if (changeInventory)
            {
                Console.WriteLine("В какой слот положить новый инструмент?");

                int input = INVALID_INDEX;
                while (true)
                {
                    Console.WriteLine("Введите '0' чтобы выйти!");
                    Int32.TryParse(Console.ReadLine(), out input);

                    if (input == 0)
                    {
                        break;
                    }

                    if (input > 0
                       && input <= p.instruments.Length)
                    {
                        p.instruments[input - 1] = GetAvailableInstrument();
                        PrintInventory();
                    }
                }
            }

            bool findCave = AskYesNo("Желаете ли найти новую пещеру ?");
            if(findCave)
            {
                AddNewCave();
            }

            SelectCaveList();
        }

        static void Initialize()
        {
            p = new Player();
            rand = new Random();

            caves = new List<Cave>();

            AddNewCave();
            AddNewCave();
            AddNewCave();
        }

        static void GoToCave(Cave cave)
        {
            Console.WriteLine($"Вы {(goToCave ? "пришли в пешеру" : "вышли из комнаты")} и обнаружили {cave.rooms.Length} комнат. В какую комнату вы пойдете ? (1 - {cave.rooms.Length})");
            goToCave = false;
            int input = INVALID_INDEX;
            while(true)
            {
                Console.WriteLine("Введите '0' чтобы вернуться на базу!");
                Int32.TryParse(Console.ReadLine(), out input);

                if(input == 0)
                {
                    goToCave = true;
                    isBase = true;
                    return;
                }

                if(input > 0
                    && input <= cave.rooms.Length)
                {
                    GoToRoom(cave.rooms[input - 1]);
                    return;
                }
                
            }
            
        }

        static void GoToRoom(Room room)
        {
            room.PrintZombies();
            room.PrintResorces();

            if(room.resources.Length == 0)
            {
                return;
            }

            int input = INVALID_INDEX;
            while (true)
            {
                Int32.TryParse(Console.ReadLine(), out input);
                if (pickaxe.health <= 0)
                {
                    bool broke = AskYesNo("Упсс, у вас была сломаная кирка. Вернуться на базу?");
                    if(broke)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Окей, ты всё равно векрнёшься на базу =)");
                    }
                }
                if(input > 0
                    && input <= room.resources.Length)
                {
                    PrintInventory();
                    ChooseInstrument();

                    Console.WriteLine($"Вы выбрали {room.resources[input - 1].name}");
                    
                    if(room.resources[input - 1].GetType() == typeof(Iron))
                    {
                        p.Iron++;
                    }
                    else if(room.resources[input - 1].GetType() == typeof(Diamond))
                    {
                        p.Diamonds++;
                    }

                    p.currentInstrument.health -= room.resources[input - 1].damage;
                    room.resources[input - 1] = new Resource();

                    return;
                }
            }
        }

        static void ChooseInstrument()
        {
            int input = INVALID_INDEX;
            while (true)
            {
                Console.WriteLine($"Выберете инструмент (1 - {p.instruments.Length})");

                Int32.TryParse(Console.ReadLine(), out input);

                if(input > 0
                    && input <= p.instruments.Length)
                {
                    p.currentInstrument = p.instruments[input - 1];
                    return;
                }                
            }
        }

        static void AddNewCave()
        {
            caves.Add(new Cave(rand));
        }

        static void SelectCaveList()
        {
            Console.WriteLine($"Вы обнаружили {caves.Count} пещер. Укажите номер пещеры, в которую хотите пойти (1 - {caves.Count})");

            int input = INVALID_INDEX;
            while(true)
            {
                Console.WriteLine("Введите '0' чтобы вернуться на базу!");

                Int32.TryParse(Console.ReadLine(), out input);

                if(input == 0)
                {
                    currentCave = INVALID_INDEX;
                    isBase = true;
                    return;
                }

                if(input > 0
                    && input <= caves.Count)
                {
                    currentCave = input - 1;
                    return;
                }
            }
        }

        static bool AskYesNo(string message)
        {
            string input = "";
            while(true)
            {
                Console.WriteLine(message + "(y/n) ?");
                input = Console.ReadLine();
                if(input == "y")
                {
                    return true;
                }
                else if(input == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Нужно 'y' или 'n'");
                }
            }
        }

        static bool PrintInventory()
        {
            Console.Write($"У вас {p.Iron} железа, ");
            Console.WriteLine($"и {p.Diamonds} алмазов");
            if (p.instruments.Length == 0)
            {
                Console.WriteLine("Ваш инвентарь пустой!");
                return false;
            }
            else
            {
                Console.WriteLine("В вашем инвентаре сейчас:");

                for (int i = 0; i < p.instruments.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}] {p.instruments[i].name} [{p.instruments[i].health}]");
                }
            }

            return true;
        }

        static Instrument SelectInstrument()
        {
            bool hasInstruments = PrintInventory();
            if(!hasInstruments)
            {
                return null;
            }

            return SelectInstrument(p.instruments);
        }

        static Instrument GetAvailableInstrument()
        {
            Instrument[] instruments = new Instrument[3];
            instruments[0] = new Sword();
            instruments[1] = new Pickaxe();
            instruments[2] = new Shovel();

            Console.WriteLine("Список доступных инструметов: ");

            for(int i = 0; i < instruments.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {instruments[i].name}!");
            }

            return SelectInstrument(instruments);
        }

        static Instrument SelectInstrument(Instrument[] intstruments)
        {
            int input = INVALID_INDEX;
            while (true)
            {
                Int32.TryParse(Console.ReadLine(), out input);
                if (input > 0
                   && input <= intstruments.Length)
                {
                    Console.WriteLine($"Вы выбрали {intstruments[input - 1].name}!");
                    return intstruments[input - 1];
                }
            }
        }
    }

    class Zombie
    {
        public int health = 100;
        public int damage = 5;
    }

    class Player
    {
        public int Iron = 0;
        public int Diamonds = 0;
        public int health = 100; 
        public Instrument[] instruments;
        public Instrument currentInstrument;

        public Player()
        {
            int size = 2;
            instruments = new Instrument[size];

            for(int i = 0; i < size; i++)
            {
                instruments[i] = new Instrument();
            }
        }

        public Instrument TakeFirstInstrument()
        {
            return instruments[0];
        }

        public Instrument TakeSecondInstrument()
        {
            return instruments[1];
        }
    }

    class Instrument
    {
        public string name = "empty";
        public int health;
        public int damageEnemy;
        public int damageResource;
    }

    class Sword : Instrument
    {
        public Sword()
        {
            name = "Sword";
            health = 30;
            damageEnemy = 30;
            damageResource = 10;
        }
    }

    class Pickaxe : Instrument
    {
        public Pickaxe()
        {
            name = "Pickaxe";
            health = 60;
            damageEnemy = 10;
            damageResource = 100;
        }
    }

    class Shovel : Instrument
    {
        public Shovel()
        {
            name = "Shovel";
            health = 60;
            damageEnemy = 10;
            damageResource = 100;
        }
    }

    class Cave
    {
        public Room[] rooms;
        public Cave(Random rnd)
        {
            int size = rnd.Next(4) + 2;

            rooms = new Room[size];
            for(int i = 0; i < size; i++)
            {
                rooms[i] = new Room(rnd);
            }
        }
    }

    class Room
    {
        public Resource[] resources;
        public Zombie[] zombies;       
        public Room(Random rnd)
        {
            // generate resources
            int size = rnd.Next(4);

            const int allrand = 100;
            const int randIron = 80;

            resources = new Resource[size];
            for(int i = 0; i < size; i++)
            {
                if (rnd.Next(allrand) <= randIron)
                {
                    resources[i] = new Iron();
                }
                else
                {
                    resources[i] = new Diamond();
                }
            }

            // generate zombies
            int zombieSize = rnd.Next(2);
            zombies = new Zombie[zombieSize];            
        }

        public void PrintZombies()
        {
            if(zombies.Length == 0)
            {
                Console.WriteLine("Вам повезло, в этой комнате нет зомбаков.");
                return;
            }

            Console.WriteLine($"В этой комнате {zombies.Length} зомби.");
        }

        public void PrintResorces()
        {
            if(resources.Length == 0)
            {
                Console.WriteLine("В этой комнате нет полезных ресурсов!");
                return;
            }

            Console.WriteLine("В этой комнате вы нашли: ");
            for(int i = 0; i < resources.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {resources[i].name}");
            }
        }      
    }

    class Resource
    {
        public string name = "empty";
        public int health;
        public int damage = 0;
    }

    class Iron : Resource
    {
        public Iron()
        {
            name = "Iron";
            health = 20;
            damage = 10;
        }
    }

    class Diamond : Resource
    {
        public Diamond()
        {
            name = "Diamond";
            health = 30;
            damage = 20;
        }
    }
}
