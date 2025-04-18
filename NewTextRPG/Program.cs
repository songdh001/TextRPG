using System.Numerics;
using System.Text.Json;

namespace NewTextRPG
{
    internal class Program
    {
        class Player
        {
            public string Name;
            public JobType Job = JobType.Warrior;
            public int Level = 1;

            // 기본 능력치
            public int BaseAttack = 10;
            public int BaseDefense = 5;
            public int MaxHP = 100;
            public int CurrentHP = 100;
            public int Gold = 1500;

            //아이템 장착으로 변한 능력치
            public int BonusAttack => EquippedItems.Sum(item => item.AttackBonus);
            public int BonusDefense => EquippedItems.Sum(item => item.DefenseBonus);

            //최종 능력치
            public int TotalAttack => BaseAttack + BonusAttack;
            public int TotalDefense => BaseDefense + BonusDefense;

            public List<Item> Inventory = new List<Item>();
            public List<Item> EquippedItems = new List<Item>();

            //직업에 따라 능력치 조절해주기 전사가 기본 스텟이니까 도적과 마법사만 조절
            //밸런스 공격력 1당 최대체력 -5, 방여력 -0.5(내림)
            public void ApplyJobStats()
            {
                switch (Job)
                {
                    case JobType.Thief:
                        BaseAttack += 2;
                        BaseDefense -= 1;
                        MaxHP -= 10;
                        break;
                    case JobType.Mage:
                        BaseAttack += 5;
                        BaseDefense -= 2;
                        MaxHP -= 25;
                        break;
                }

                CurrentHP = MaxHP;
            }
        }

        class Item
        {
            public string Name { get; set; }
            public int AttackBonus { get; set; }
            public int DefenseBonus { get; set; }
            public int MaxHP { get; set; }
            public int Price { get; set; }


            // 역직렬화를 위한 기본 생성자 이걸 해놓아야 저장 데이터가 로드 됨
            public Item() { }


            public Item(string name, int atk, int def, int HP, int price)
            {
                Name = name;
                AttackBonus = atk;
                DefenseBonus = def;
                MaxHP = HP;
                Price = price;
            }
            public override string ToString()
            {
                return $"{Name} (공격력 +{AttackBonus}, 방어력 +{DefenseBonus}, 최대체력 +{MaxHP}, 가격: {Price}G)";
            }

        }
        enum JobType
        {
            Warrior,    // 전사
            Thief,      // 도적
            Mage        // 마법사
        }



        static Player player = new Player();

        static void Main(string[] args)
        {
            Console.WriteLine("1) 새 게임 시작");
            Console.WriteLine("2) 저장된 게임 불러오기");
            Console.Write("선택: ");
            string input = Console.ReadLine();

            player = new Player();
            if (input == "2")
            {
                //세이브 데이터 로드
                LoadGame();
            }
            else
            {
                //캐릭터 생성 창으로 이동
                StartGame();
            }

            //메인 화면으로 이동
            MainMenu();
        }

        static void StartGame()
        {

            //시작하고 캐릭터 이름 설정
            Console.WriteLine("===== 텍스트 RPG에 오신 것을 환영합니다! =====");
            Console.Write("당신의 캐릭터 이름을 입력하세요: ");
            player.Name = Console.ReadLine();


            //캐릭터 이름 정하고 직업 설정
            Console.WriteLine("\n직업을 선택하세요:");
            Console.WriteLine("1) 전사 (밸런스 있는 능력치)");
            Console.WriteLine("2) 도적 (강력하지만 낮은 체력)");
            Console.WriteLine("3) 마법사 (매우 강력하지만 허약한 체력)");

            while (true)
            {
                Console.Write("번호 입력: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        player.Job = JobType.Warrior;
                        break;
                    case "2":
                        player.Job = JobType.Thief;
                        break;
                    case "3":
                        player.Job = JobType.Mage;
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
                        continue;
                }
                break;
            }
            //직업 고르면 능력치 조정
            player.ApplyJobStats();

            Console.WriteLine($"\n{player.Name}님, {player.Job}로 모험을 시작합니다!\n");

        }

        static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("======= 메인 메뉴 =======");
                Console.WriteLine("1) 캐릭터 정보");
                Console.WriteLine("2) 인벤토리");
                Console.WriteLine("3) 상점");
                Console.WriteLine("4) 던전 도전");
                Console.WriteLine("5) 여관");
                Console.WriteLine("6) 저장");
                Console.WriteLine("0) 게임 종료");
                Console.Write("선택: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        PlayerStat();
                        break;
                    case "2":
                        Inventory();
                        break;
                    case "3":
                        Shop();
                        break;
                    case "4":
                        Dungeon();
                        break;
                    case "5":
                        Inn();
                        break;
                    case "6":
                        SaveGame();
                        break;
                    case "0":
                        Console.WriteLine("게임을 종료합니다.");
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.\n");
                        break;
                }
            }            
        }

        static void PlayerStat()
        {
            Console.WriteLine("\n===== 캐릭터 정보 =====");
            Console.WriteLine($"이름: {player.Name}");
            Console.WriteLine($"직업: {player.Job}");
            Console.WriteLine($"레벨: {player.Level}");

            Console.WriteLine($"공격력: {player.BaseAttack} (+{player.BonusAttack}) => {player.TotalAttack}");
            Console.WriteLine($"방어력: {player.BaseDefense} (+{player.BonusDefense}) => {player.TotalDefense}");

            Console.WriteLine($"체력: {player.CurrentHP} / {player.MaxHP}");
            Console.WriteLine($"Gold: {player.Gold}");

            Console.WriteLine("\n[장착 중인 아이템]");
            if (player.EquippedItems.Count == 0)
            {
                Console.WriteLine("없음");
            }
            else
            {
                foreach (var item in player.EquippedItems)
                {
                    Console.WriteLine($"- {item}");
                }
            }

            Console.WriteLine("========================\n");
        }

        static void Inventory()
        {
            Console.WriteLine("\n===== 인벤토리 =====");

            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("인벤토리가 비어 있습니다.");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    var item = player.Inventory[i];
                    Console.WriteLine($"{i + 1}) {item.Name} (공격력 +{item.AttackBonus}, 방어력 +{item.DefenseBonus}, 최대체력 +{item.MaxHP})");
                }
            }

            Console.WriteLine("\n장착 중인 아이템:");
            if (player.EquippedItems.Count == 0)
            {
                Console.WriteLine("장착된 아이템이 없습니다.");
            }
            else
            {
                foreach (var item in player.EquippedItems)
                {
                    Console.WriteLine($"- [E] {item.Name} (공격력 +{item.AttackBonus}, 방어력 +{item.DefenseBonus}, 최대체력 +{item.MaxHP})");
                }
            }

            Console.WriteLine("====================\n");
            //장착, 해체는 다른 함수로 부르기
            ManageInventory();
        }

        static void ManageInventory()
        {
            Console.WriteLine("1) 아이템 장착");
            Console.WriteLine("2) 아이템 해제");
            Console.WriteLine("0) 돌아가기");
            Console.Write("선택: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    EquipItem();
                    break;
                case "2":
                    UnequipItem();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    break;
            }
        }

        static void EquipItem()
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("장착할 아이템이 없습니다.");
                return;
            }

            Console.Write("장착할 아이템 번호를 입력하세요: ");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index < 1 || index > player.Inventory.Count)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    return;
                }

                var item = player.Inventory[index - 1];
                player.EquippedItems.Add(item);
                player.Inventory.RemoveAt(index - 1);
                Console.WriteLine($"{item.Name}을(를) 장착했습니다.");
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }

        static void UnequipItem()
        {
            if (player.EquippedItems.Count == 0)
            {
                Console.WriteLine("해제할 아이템이 없습니다.");
                return;
            }

            Console.Write("해제할 아이템 번호를 입력하세요: ");
            for (int i = 0; i < player.EquippedItems.Count; i++)
            {
                var item = player.EquippedItems[i];
                Console.WriteLine($"{i + 1}) {item.Name} (공격력 +{item.AttackBonus}, 방어력 +{item.DefenseBonus}, 최대체력 +{item.MaxHP})");
            }

            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index < 1 || index > player.EquippedItems.Count)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    return;
                }

                var item = player.EquippedItems[index - 1];
                player.Inventory.Add(item);
                player.EquippedItems.RemoveAt(index - 1);
                Console.WriteLine($"{item.Name}을(를) 해제했습니다.");
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }

        //상점에서 팔 거 리스트
        //밸런스 공격력, 방어력 1 당 100, 최대체력 10당 200
        static List<Item> shopItems = new List<Item>
        {
                new Item("철검", 10, 0, 0, 1000),
                new Item("갑옷", 0, 10, 10, 1200),
                new Item("지팡이", 15, -2, 0, 1300),
                new Item("도적의 방패", 2, 10, 20 , 1600),
                new Item("전사의 방패", 10, 5, 20 , 1900),
                new Item("마법사의 지팡이", 20, 0, 10 , 2100)
        };


        static void Shop()
        {
            while (true)
            {
                Console.WriteLine("\n===== 상점 =====");
                Console.WriteLine("1) 아이템 구매");
                Console.WriteLine("2) 아이템 판매");
                Console.WriteLine("0) 나가기");
                Console.Write("선택: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        BuyItem();
                        break;
                    case "2":
                        SellItem();
                        break;
                    case "0":
                        Console.WriteLine("상점을 나갑니다.\n");
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        break;
                }
            }
        }

        static void BuyItem()
        {
            Console.WriteLine("\n===== 상점 아이템 목록 =====");
            for (int i = 0; i < shopItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {shopItems[i]}");
            }

            Console.WriteLine($"보유 Gold: {player.Gold}");
            Console.Write("구매할 아이템 번호 입력 (0: 취소): ");

            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index == 0)
                    return;

                if (index < 1 || index > shopItems.Count)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    return;
                }

                Item selected = shopItems[index - 1];
                if (player.Gold >= selected.Price)
                {
                    player.Gold -= selected.Price;
                    player.Inventory.Add(selected);
                    Console.WriteLine($"{selected.Name}을(를) 구매했습니다.");
                }
                else
                {
                    Console.WriteLine("Gold가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }

        static void SellItem()
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("판매할 아이템이 없습니다.");
                return;
            }

            Console.WriteLine("\n===== 판매 가능한 아이템 =====");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Item item = player.Inventory[i];
                Console.WriteLine($"{i + 1}) {item} (판매가: {item.Price / 2}G)");
            }

            Console.Write("판매할 아이템 번호 입력 (0: 취소): ");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index == 0)
                    return;

                if (index < 1 || index > player.Inventory.Count)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    return;
                }

                Item item = player.Inventory[index - 1];
                player.Gold += item.Price / 2;
                player.Inventory.RemoveAt(index - 1);
                Console.WriteLine($"{item.Name}을(를) 판매했습니다. {item.Price / 2}G 획득!");
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }


        //던전에 나올 몬스터 클래스

        class Monster
        {
            public string Name;
            public int HP;
            public int Attack;
            public int Defense;
            public int RewardGold;

            public Monster(string name, int hp, int atk, int def, int reward)
            {
                Name = name;
                HP = hp;
                Attack = atk;
                Defense = def;
                RewardGold = reward;
            }
        }


        static void Dungeon()
        {
            Console.WriteLine("\n===== 던전 선택 =====");
            Console.WriteLine("1) 쉬움");
            Console.WriteLine("2) 보통");
            Console.WriteLine("3) 어려움");
            Console.Write("난이도를 선택하세요: ");
            string input = Console.ReadLine();

            Monster monster;

            switch (input)
            {
                case "1":
                    monster = new Monster("고블린", 30, 5, 1, 200);
                    break;
                case "2":
                    monster = new Monster("오크", 50, 10, 3, 300);
                    break;
                case "3":
                    monster = new Monster("드래곤", 100, 20, 5, 700);
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    return;
            }
            //전투 페이즈는 다른 함수로 뺌
            StartBattle(monster);
        }

        static void StartBattle(Monster monster)
        {
            Console.WriteLine($"\n{monster.Name}이(가) 나타났다!");

            while (monster.HP > 0 && player.CurrentHP > 0)
            {
                Console.WriteLine($"\n[내 체력: {player.CurrentHP} / {player.MaxHP}]");
                Console.WriteLine($"[{monster.Name} 체력: {monster.HP}]");
                Console.WriteLine("1) 공격하기  2) 도망가기");
                Console.Write("선택: ");

                string input = Console.ReadLine();

                if (input == "1")
                {
                    // 플레이어 공격
                    int damageToMonster = Math.Max(1, player.TotalAttack - monster.Defense);
                    monster.HP -= damageToMonster;
                    Console.WriteLine($"{monster.Name}에게 {damageToMonster}의 피해를 입혔습니다!");

                    if (monster.HP <= 0)
                    {
                        Console.WriteLine($"{monster.Name}을(를) 물리쳤습니다!");
                        Console.WriteLine($"{monster.RewardGold}G를 획득했습니다!");
                        player.Gold += monster.RewardGold;
                        //전투에서 승리하면 랜덤 이벤트 출력
                        RandomEvent();
                        return;
                    }

                    // 몬스터 반격
                    int damageToPlayer = Math.Max(1, monster.Attack - player.TotalDefense);
                    player.CurrentHP -= damageToPlayer;
                    Console.WriteLine($"{monster.Name}의 공격! {damageToPlayer}의 피해를 입었습니다!");

                    if (player.CurrentHP <= 0)
                    {
                        Console.WriteLine("당신은 패배했습니다... 여관에서 회복하세요.");
                        player.CurrentHP = 1; // 죽지는 않고 체력 1로 생존
                        return;
                    }
                }
                else if (input == "2")
                {
                    Console.WriteLine("도망쳤습니다!");
                    return;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
            }
        }

        static void RandomEvent()
        {
            Random rand = new Random();
            int eventType = rand.Next(3); // 0, 1, 2

            Console.WriteLine("\n--- 마을로 돌아가는 중에 무슨 일이 일어납니다... ---");

            switch (eventType)
            {
                case 0:
                    PieEvent();
                    break;
                case 1:
                    FoxEvent();
                    break;
                case 2:
                    CatEvent();
                    break;
            }

            Console.WriteLine("------------------------------\n");
        }

        static void PieEvent()
        {
            string PieArt = @"
         (
          )
     __..---..__
 ,-='  /  |  \  `=-.
:--..___________..--;
 \.,_____________,./
";
            Console.WriteLine(PieArt);
            Console.WriteLine("길에 맛있어 보이는 파이가 놓여 있습니다. 먹어볼까요?");
            Console.Write("1) 먹는다  2) 그냥 간다 → ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                Random rand = new Random();
                int chance = rand.Next(100);
                if (chance < 80) // 80% 확률 회복
                {
                    int heal = rand.Next(10, 21);
                    player.CurrentHP = Math.Min(player.MaxHP, player.CurrentHP + heal);
                    Console.WriteLine($"파이를 먹고 체력을 {heal} 회복했습니다!");
                }
                else
                {
                    int dmg = rand.Next(5, 16);
                    player.CurrentHP = Math.Max(1, player.CurrentHP - dmg);
                    Console.WriteLine($"상한 파이였습니다... 체력이 {dmg} 감소했습니다!");
                }
            }
            else
            {
                Console.WriteLine("파이를 무시하고 지나갑니다.");
            }
        }

        static void FoxEvent()
        {
            string FoxArt = @"
   /|_/|
  / ^ ^(_o
 /    __.'
 /     \
(_) (_) '._
  '.__     '. .-''-'.
     ( '.   ('.____.''
     _) )'_, )mrf
    (__/ (__/
";
            Console.WriteLine(FoxArt);
            Console.WriteLine("수상한 여우가 당신을 따라오라고 합니다.");
            Console.Write("1) 따라간다  2) 무시한다 → ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                Random rand = new Random();
                int chance = rand.Next(100);

                int gold = rand.Next(10, 31);
                player.Gold += gold;
                Console.WriteLine($"여우를 따라갔다가 {gold}G를 발견했습니다!");

                if (chance < 30)
                {
                    int hpLoss = rand.Next(5, 16);
                    player.CurrentHP = Math.Max(1, player.CurrentHP - hpLoss);
                    Console.WriteLine($"그러나 덤불에 찔려 체력이 {hpLoss} 감소했습니다...");
                }
            }
            else
            {
                Console.WriteLine("여우를 무시하고 길을 갑니다.");
            }
        }

        static void CatEvent()
        {
            string CatsArt = @"
                      /^--^\     /^--^\     /^--^\
                      \____/     \____/     \____/
                     /      \   /      \   /      \
                    |        | |        | |        |
                     \__  __/   \__  __/   \__  __/
|^|^|^|^|^|^|^|^|^|^|^|^\ \^|^|^|^/ /^|^|^|^|^\ \^|^|^|^|^|^|^|^|^|^|^|^|
| | | | | | | | | | | | |\ \| | |/ /| | | | | | \ \ | | | | | | | | | | |
########################/ /######\ \###########/ /#######################
| | | | | | | | | | | | \/| | | | \/| | | | | |\/ | | | | | | | | | | | |
|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|_|
";
            Console.WriteLine(CatsArt);
            Console.WriteLine("울타리 위에 세 마리의 고양이가 앉아 있습니다.");
            Console.WriteLine("1) 첫 번째 고양이  2) 두 번째 고양이  3) 세 번째 고양이");
            Console.Write("하나를 골라 쓰다듬습니다 → ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 3)
            {
                Random rand = new Random();
                int outcome = rand.Next(3); // 0: 체력 회복, 1: 골드, 2: 아무 일도 없음

                switch (outcome)
                {
                    case 0:
                        int heal = rand.Next(10, 21);
                        player.CurrentHP = Math.Min(player.MaxHP, player.CurrentHP + heal);
                        Console.WriteLine("고양이가 만족스럽게 울었습니다. 체력을 " + heal + " 회복했습니다!");
                        break;
                    case 1:
                        int gold = rand.Next(10, 31);
                        player.Gold += gold;
                        Console.WriteLine("고양이 목에 걸린 주머니를 발견! " + gold + "G를 얻었습니다!");
                        break;
                    case 2:
                        Console.WriteLine("고양이는 관심이 없어 보입니다... 아무 일도 일어나지 않았습니다.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다. 고양이들은 당신을 외면했습니다.");
            }
        }

        // 여관 이용 비용
        const int InnHealCost = 30; 
        static void Inn()
        {
            Console.WriteLine("\n===== 여관 =====");
            Console.WriteLine($"체력을 회복하시겠습니까? ({InnHealCost} Gold 소모)");
            Console.WriteLine($"현재 체력: {player.CurrentHP} / {player.MaxHP}");
            Console.WriteLine($"보유 Gold: {player.Gold}");
            Console.Write("1) 회복한다   0) 나간다 → ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                if (player.CurrentHP == player.MaxHP)
                {
                    Console.WriteLine("이미 체력이 가득 찼습니다.");
                }
                else if (player.Gold < InnHealCost)
                {
                    Console.WriteLine("Gold가 부족합니다.");
                }
                else
                {
                    player.Gold -= InnHealCost;
                    player.CurrentHP = player.MaxHP;
                    Console.WriteLine("체력을 모두 회복했습니다!");
                }
            }
            else if (input == "0")
            {
                Console.WriteLine("여관을 나갑니다.");
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Console.WriteLine();
        }


        //플레이어 데이터 저장하기

        class PlayerData
        {
            public string Name { get; set; }
            public string Job { get; set; }
            public int Level { get; set; }
            public int BaseAttack { get; set; }
            public int BaseDefense { get; set; }
            public int MaxHP { get; set; }
            public int CurrentHP { get; set; }
            public int Gold { get; set; }

            public List<Item> Inventory { get; set; } = new List<Item>();
            public List<Item> EquippedItems { get; set; } = new List<Item>();
        }

        static void SaveGame()
        {
            var data = new PlayerData
            {
                Name = player.Name,
                Job = player.Job.ToString(),
                Level = player.Level,
                BaseAttack = player.BaseAttack,
                BaseDefense = player.BaseDefense,
                MaxHP = player.MaxHP,
                CurrentHP = player.CurrentHP,
                Gold = player.Gold,
                Inventory = player.Inventory,
                EquippedItems = player.EquippedItems
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("저장할 JSON 데이터:\n" + json);
            File.WriteAllText("save.json", json);
            Console.WriteLine("\n게임이 저장되었습니다!\n");
        }

        static void LoadGame()
        {
            if (!File.Exists("save.json"))
            {
                Console.WriteLine("저장된 게임이 없습니다.");
                //저장된 데이터가 없으니 새 게임 시작해서 캐릭터 만들어줌
                StartGame();
                return;
            }

            string json = File.ReadAllText("save.json");
            var data = JsonSerializer.Deserialize<PlayerData>(json);

            player.Name = data.Name;
            player.Job = Enum.Parse<JobType>(data.Job);
            player.Level = data.Level;
            player.BaseAttack = data.BaseAttack;
            player.BaseDefense = data.BaseDefense;
            player.MaxHP = data.MaxHP;
            player.CurrentHP = data.CurrentHP;
            player.Gold = data.Gold;
            player.Inventory = data.Inventory;
            player.EquippedItems = data.EquippedItems;

            Console.WriteLine("\n게임을 불러왔습니다!\n");
        }
    }
}
