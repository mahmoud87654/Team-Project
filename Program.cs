using System.Globalization;

class Product
{
    public string Name { get; set; }
    public string Cat { get; set; }
    public int Quan { get; set; }
    public double Pric { get; set; } 
    public DateTime ProDat { get; set; }
    public DateTime EXData { get; set; }

    public void Display()
    {
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Category: {Cat}");
        Console.WriteLine($"Quantity: {Quan}");
        Console.WriteLine($"Price: {Pric:F2} EGP");
        Console.WriteLine("Production Date: " + ProDat.ToString("dd/MM/yyyy"));
        Console.WriteLine("Expiry Date: " + EXData.ToString("dd/MM/yyyy"));
        Console.WriteLine("---------------------------------");
    }
}

/// خانة اللي هيتشري منتجاتنا
class Customer
{
    public string Name { get; set; }
    public List<Product> Cart { get; set; } = new List<Product>();

    public void BuyProduct(Product p, int quantity)
    {
        if (quantity <= 0)
        {
            Console.WriteLine("❌ Quantity must be positive.");
            return;
        }

        if (p.Quan >= quantity)
        {
            p.Quan -= quantity;
            Cart.Add(new Product
            {
                Name = p.Name,
                Cat = p.Cat,
                Quan = quantity,
                Pric = p.Pric,
                ProDat = p.ProDat,
                EXData = p.EXData
            });
            Console.WriteLine($"✅ {quantity} of {p.Name} added to {Name}'s cart.");
        }
        else
        {
            Console.WriteLine("❌ Not enough quantity available.");
        }
    }

    public void ShowCart()
    {
        if (Cart.Count == 0)
        {
            Console.WriteLine($"\n🛒 {Name}'s Cart is empty.");
            return;
        }

        Console.WriteLine($"\n🛒 {Name}'s Cart:");
        double total = 0;
        foreach (var item in Cart)
        {
            Console.WriteLine($"- {item.Name} | Qty: {item.Quan} | Price: {item.Pric:F2} EGP | Subtotal: {(item.Pric * item.Quan):F2} EGP");
            total += item.Pric * item.Quan;
        }
        Console.WriteLine($"💰 Total Price: {total:F2} EGP");
    }
}

class Program
{
    static List<Product> products = new List<Product>();
    static List<Customer> customers = new List<Customer>();

    /// اختصاراا للوقت هنا لازم يكون في يوزر نيم و باس 
    /// اليوزر نيم هو : admin
    /// ده اللي بييكون ماسك السوبر ماركت 
    // الكاشير من الاخر
    /// وال باس ثابت
    // ====== Config ======
    const string user = "admin";
    const string ADMIN_PASS = "1911";
    // const لل low stock عشان لو في 5 يطلع تحذير
    const int LOW_STOCK_THRESHOLD = 5;
    // نفس الكلام بس بدل م هنقول low stockk هنقولل الايام اللي قاضلة ع المنتج عشان يبووظظ
    const int NEAR_EXPIRY_DAYS = 5;

    static void Main(string[] args)
    {

        // زي مقولنا اللغات
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.WriteLine("\n====== Main Menu ======");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Customer");
            Console.WriteLine("3. Exit");
            Console.Write("Choose: ");
            string x = Console.ReadLine();

            switch (x)
            {
                case "1":
                    if (Log())
                        MenuAdmin();
                    break;
                case "2":
                    CustomerMenu();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("❌ Invalid choice.");
                    break;
            }
        }
    }

    // ====== Admin ======
    static bool Log()
    {
        Console.Write("\nUsername: ");
        string u = Console.ReadLine();
        Console.Write("Password: ");
        string p = ReadPassword();  // نجوم الباس

        if (u == user && p == ADMIN_PASS)
        {
            Console.WriteLine("✅ Login successful.");
            return true;
        }
        Console.WriteLine("❌ Invalid credentials.");
        return false;
    }

    static void MenuAdmin()
    {
        while (true)
        {
            Console.WriteLine("\n====== Admin Menu ======");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View Products");
            Console.WriteLine("3. Stock & Expiry Alerts");      // (4 + 7)
            Console.WriteLine("4. Filter by Category");         // (6)
            Console.WriteLine("5. Back");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    ViewProducts();
                    break;
                case "3":
                    ViewAlerts();
                    break;
                case "4":
                    FilterCat();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("❌ Invalid choice.");
                    break;
            }
        }
    }

    static void AddProduct()
    {
        string name;
        while (true)
        {
            Console.Write("Enter product name (letters only): ");
            name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name) && IsLettersOnly(name)) break;
            Console.WriteLine("❌ Invalid name! Please enter letters only (no numbers or symbols).");
        }

        string category;
        while (true)
        {
            Console.Write("Enter category (letters only): ");
            category = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(category) && IsLettersOnly(category)) break;
            Console.WriteLine("❌ Invalid category! Please enter letters only (no numbers or symbols).");
        }

        int quantity;
        Console.Write("Enter quantity: ");
        while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
        {
            Console.WriteLine("❌ Invalid quantity! Must be a positive number.");
            Console.Write("Enter quantity: ");
        }

        double price;
        Console.Write("Enter price (EGP): ");
        while (!double.TryParse(Console.ReadLine(), out price) || price <= 0)
        {
            Console.WriteLine("❌ Invalid price! Must be a positive number.");
            Console.Write("Enter price (EGP): ");
        }

        DateTime productionDate = ReadDate("Enter production date (d/M/yyyy or d-M-yyyy): ");
        DateTime expiryDate = ReadDate("Enter expiry date (d/M/yyyy or d-M-yyyy): ");

                // بنتشيك لو التاريخ المنتج صح ولا لا 
        if (expiryDate <= productionDate)
        {
            Console.WriteLine("❌ Expiry date must be after production date.");
            return;
        }

        products.Add(new Product
        {
            Name = name,
            Cat = category,
            Quan = quantity,
            Pric = price,
            ProDat = productionDate,
            EXData = expiryDate
        });

        Console.WriteLine("✅ Product added successfully!");
    }

    // عرض المنتجات 
    static void ViewProducts()
    {
        if (products.Count == 0)
        {
            Console.WriteLine("❌ No products available.");
            return;
        }

        Console.WriteLine("\n📦 Product List:");
        foreach (var p in products)
        {
            p.Display();
        }
    }

    // انذارات وتحذيرات المنتج وعددهم
    static void ViewAlerts()
    {
        if (products.Count == 0)
        {
            Console.WriteLine("❌ No products available.");
            return;
        }

        Console.WriteLine("\n⚠️ Stock & Expiry Alerts:");
        DateTime today = DateTime.Now;
        bool any = false;

        foreach (var p in products)
        {
            bool lowStock = p.Quan <= LOW_STOCK_THRESHOLD;
            bool expired = p.EXData.Date < today.Date;
            bool nearExpiry = !expired && (p.EXData.Date - today.Date).TotalDays <= NEAR_EXPIRY_DAYS;

            if (lowStock || expired || nearExpiry)
            {
                any = true;
                Console.WriteLine($"- {p.Name} | Qty: {p.Quan} | Expiry: {p.EXData:dd/MM/yyyy}");
                if (lowStock) Console.WriteLine("  • 🔻 Low stock alert");
                if (expired) Console.WriteLine("  • ⛔ EXPIRED");
                else if (nearExpiry) Console.WriteLine($"  • ⏳ Near expiry (≤ {NEAR_EXPIRY_DAYS} days)");
            }
        }

        if (!any) Console.WriteLine("✅ No alerts. All good!");
    }

    // هنا بنفلتر المنتجات حسب ال صنف بتاعهم
    static void FilterCat()
    {
        if (products.Count == 0)
        {
            Console.WriteLine("❌ No products available.");
            return;
        }

        string cat;
        while (true)
        {
            Console.Write("Enter category to filter (letters only): ");
            cat = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cat) && IsLettersOnly(cat)) break;
            Console.WriteLine("❌ Invalid category! Letters only.");
        }

        var matches = products.FindAll(p =>
            p.Cat.Equals(cat, StringComparison.OrdinalIgnoreCase));

        if (matches.Count == 0)
        {
            Console.WriteLine("⚠️ No products found in this category.");
            return;
        }

        Console.WriteLine($"\n📂 Products in category: {cat}");
        foreach (var p in matches) p.Display();
    }

    // ====== Customer ======
    static void CustomerMenu()
    {
        string cname;
        while (true)
        {
            Console.Write("Enter your name (letters only): ");
            cname = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cname) && IsLettersOnly(cname)) break;
            Console.WriteLine("❌ Invalid name! Please enter letters only (no numbers or symbols).");
        }

        Customer c = customers.Find(x => x.Name.Equals(cname, StringComparison.OrdinalIgnoreCase));
        if (c == null)
        {
            c = new Customer { Name = cname };
            customers.Add(c);
        }

        while (true)
        {
            Console.WriteLine($"\n====== Customer Menu ({c.Name}) ======");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Filter by Category");  // (6)
            Console.WriteLine("3. Buy Product");
            Console.WriteLine("4. View My Cart");
            Console.WriteLine("5. Checkout / Payment");  // (8)
            Console.WriteLine("6. Back");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewProducts();
                    break;

                case "2":
                    FilterCat();
                    break;

                case "3":
                    BuyFlow(c);
                    break;

                case "4":
                    c.ShowCart();
                    break;

                case "5":
                    Checkout(c);
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("❌ Invalid choice.");
                    break;
            }
        }
    }
    /// الشراء من العميل
    static void BuyFlow(Customer c)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("❌ No products available.");
            return;
        }

        ViewProducts();
        Console.Write("Enter product name: ");
        string pname = Console.ReadLine();

        Product p = products.Find(x => x.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
        if (p == null)
        {
            Console.WriteLine("❌ Product not found.");
            return;
        }

        Console.Write("Enter quantity: ");
        int q;
        while (!int.TryParse(Console.ReadLine(), out q) || q <= 0)
        {
            Console.WriteLine("❌ Invalid quantity! Must be a positive number.");
            Console.Write("Enter quantity: ");
        }

        c.BuyProduct(p, q);
    }

    // عملية التشيييييك
    static void Checkout(Customer c)
    {
        if (c.Cart.Count == 0)
        {
            Console.WriteLine("🛒 Your cart is empty.");
            return;
        }

        c.ShowCart();
        double total = 0;
        foreach (var item in c.Cart) total += item.Pric * item.Quan;

        Console.WriteLine("\n💳 Choose payment method:");
        Console.WriteLine("1. Cash");
        Console.WriteLine("2. Card");
        Console.Write("Choose: ");
        string method = Console.ReadLine();

        if (method == "1")
        {
            Console.WriteLine($"✅ Cash payment received: {total:F2} EGP");
        }
        else if (method == "2")
        {
            // شيك لعملية البطاقة عشان تكون متناسقة
            Console.Write("Enter card number (16 digits): ");
            string card = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(card) || card.Length != 16 || !IsDigitsOnly(card))
            {
                Console.WriteLine("❌ Invalid card number! Must be exactly 16 digits.");
                Console.Write("Enter card number (16 digits): ");
                card = Console.ReadLine();
            }

            Console.Write("Enter card expiry (MM/YY): ");
            string exp = Console.ReadLine();
            while (!ValidCardExpiry(exp))
            {
                Console.WriteLine("❌ Invalid expiry! Use MM/YY and a future date.");
                Console.Write("Enter card expiry (MM/YY): ");
                exp = Console.ReadLine();
            }

            Console.Write("Enter CVV (3 digits): ");
            string cvv = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !IsDigitsOnly(cvv))
            {
                Console.WriteLine("❌ Invalid CVV! Must be 3 digits.");
                Console.Write("Enter CVV (3 digits): ");
                cvv = Console.ReadLine();
            }

            Console.WriteLine($"✅ Card payment approved: {total:F2} EGP");
        }
        else
        {
            Console.WriteLine("❌ Invalid payment method.");
            return;
        }

        // الفاتورة بتاعته ) المشتري
        Console.WriteLine("\n🧾 Receipt:");
        foreach (var item in c.Cart)
            Console.WriteLine($"- {item.Name} x{item.Quan} @ {item.Pric:F2} = {(item.Pric * item.Quan):F2} EGP");
        Console.WriteLine($"Total: {total:F2} EGP");
        c.Cart.Clear();
        Console.WriteLine("🎉 Thank you for your purchase!");
    }

    // ====== Helpers ======
    static DateTime ReadDate(string message)
    {
        string[] formats = { "d/M/yyyy", "d-M-yyyy" };
        DateTime date;
        Console.Write(message);
        while (!DateTime.TryParseExact(Console.ReadLine(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            Console.WriteLine("❌ Invalid date format! Example: 23/8/2025 or 23-8-2025.");
            Console.Write(message);
        }
        return date;
    }

    // وهنا بتأكد لو التاريخ بيدخله صح ولا بيهزر!
    static bool IsLettersOnly(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsLetter(c) && c != ' ')
                return false;
        }
        return true;
    }
    // نفس اللي فوق!
    static bool IsDigitsOnly(string input)
    {
        foreach (char c in input)
            if (!char.IsDigit(c)) return false;
        return true;
    }

    static bool ValidCardExpiry(string mmYY)
    {
        if (string.IsNullOrWhiteSpace(mmYY)) return false;
        var parts = mmYY.Split('/');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out int mm) || mm < 1 || mm > 12) return false;
        if (!int.TryParse(parts[1], out int yy) || yy < 0 || yy > 99) return false;

        // نفترض سنوات من 2000 إلى 2099
        int year = 2000 + yy;
        int lastDay = DateTime.DaysInMonth(year, mm);
        var expDate = new DateTime(year, mm, lastDay, 23, 59, 59);

        return expDate > DateTime.Now;
    }

    //  النجوم بتاع الباس
    static string ReadPassword()
    {
        string pass = "";
        ConsoleKeyInfo k;
        do
        {
            k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.Enter) break;
            if (k.Key == ConsoleKey.Backspace)
            {
                if (pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(k.KeyChar))
            {
                pass += k.KeyChar;
                Console.Write("*");
            }
        } while (true);
        Console.WriteLine();
        return pass;
    }
}