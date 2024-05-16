using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab_3_Question_No_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class MenuItem : INotifyPropertyChanged
    {
        private int quantity;

        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value != quantity)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MainWindow : Window
    {

        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> CartItems { get; set; }
        
        private const decimal CanadianTaxRate = 0.13m;

        

        public MainWindow()
        {
            InitializeComponent();
            MenuItems = new ObservableCollection<MenuItem>();
            CartItems = new ObservableCollection<MenuItem>();
            LoadMenuItems();
            DataContext = this;
            subTotal.Text = "$0.00";
            Tax.Text = "$0.00";
            Total.Text = "$0.00";
        }
        private void LoadMenuItems()
        {
            
            MenuItems.Add(new MenuItem { Name = "Soda", Category = "Beverage", Price = 1.95m });
            MenuItems.Add(new MenuItem { Name = "Tea", Category = "Beverage", Price = 1.50m });
            MenuItems.Add(new MenuItem { Name = "Coffee", Category = "Beverage", Price = 1.25m });
            MenuItems.Add(new MenuItem { Name = "Mineral Water", Category = "Beverage", Price = 2.95m });
            MenuItems.Add(new MenuItem { Name = "Juice", Category = "Beverage", Price = 2.50m });
            MenuItems.Add(new MenuItem { Name = "Milk", Category = "Beverage", Price = 1.50m });

            MenuItems.Add(new MenuItem { Name = "Buffalo Wings", Category = "Appetizer", Price = 5.95m });
            MenuItems.Add(new MenuItem { Name = "Buffalo Fingers", Category = "Appetizer", Price = 6.95m });
            MenuItems.Add(new MenuItem { Name = "Potato Skins", Category = "Appetizer", Price = 8.95m });
            MenuItems.Add(new MenuItem { Name = "Nachos", Category = "Appetizer", Price = 8.95m });
            MenuItems.Add(new MenuItem { Name = "Mushroom Caps", Category = "Appetizer", Price = 10.95m });
            MenuItems.Add(new MenuItem { Name = "Shrimp Cocktail", Category = "Appetizer", Price = 12.95m });
            MenuItems.Add(new MenuItem { Name = "Chips and Salsa", Category = "Appetizer", Price = 6.95m });

            MenuItems.Add(new MenuItem { Name = "Chicken Alfredo", Category = "Main Course", Price = 13.95m });
            MenuItems.Add(new MenuItem { Name = "Chicken Picatta", Category = "Main Course", Price = 13.95m });
            MenuItems.Add(new MenuItem { Name = "Turkey Club", Category = "Main Course", Price = 11.95m });
            MenuItems.Add(new MenuItem { Name = "Lobster Pie", Category = "Main Course", Price = 19.95m });
            MenuItems.Add(new MenuItem { Name = "Prime Rib", Category = "Main Course", Price = 20.95m });
            MenuItems.Add(new MenuItem { Name = "Shrimp Scampi", Category = "Main Course", Price = 18.95m });
            MenuItems.Add(new MenuItem { Name = "Turkey Dinner", Category = "Main Course", Price = 13.95m });
            MenuItems.Add(new MenuItem { Name = "Stuffed Chicken", Category = "Main Course", Price = 14.95m });
            MenuItems.Add(new MenuItem { Name = "Seafood Alfredo", Category = "Main Course", Price = 15.95m });

            MenuItems.Add(new MenuItem { Name = "Apple Pie", Category = "Dessert", Price = 5.95m });
            MenuItems.Add(new MenuItem { Name = "Sundae", Category = "Dessert", Price = 3.95m });
            MenuItems.Add(new MenuItem { Name = "Carrot Cake", Category = "Dessert", Price = 5.95m });
            MenuItems.Add(new MenuItem { Name = "Apple Crisp", Category = "Dessert", Price = 5.95m });
            MenuItems.Add(new MenuItem { Name = "Mud Pie", Category = "Dessert", Price = 4.95m });

            var categories = MenuItems.Select(item => item.Category).Distinct().ToList();
            foreach (var cat in categories)
            {
                category.Items.Add(cat);
            }

        }

        private void category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (category.SelectedItem != null)
            {
                string selectedCategory = category.SelectedItem.ToString();
                List<MenuItem> filteredItems = MenuItems.Where(item => item.Category == selectedCategory).ToList();

                Items.ItemsSource = filteredItems.Select(item => item.Name);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (Items.SelectedItem != null)
            {
                string selectedItemName = Items.SelectedItem.ToString();
                MenuItem selectedItem = MenuItems.FirstOrDefault(item => item.Name == selectedItemName);

                if (selectedItem != null)
                {
                    bool itemExistsInCart = CartItems.Any(item => item.Name == selectedItem.Name);
                    if (itemExistsInCart)
                    {
                        MenuItem existingItem = CartItems.FirstOrDefault(item => item.Name == selectedItem.Name);
                        if (existingItem != null)
                        {
                            existingItem.Quantity++;
                            UpdatePayments(); 
                        }
                    }
                    else
                    {
                        CartItems.Add(new MenuItem
                        {
                            Name = selectedItem.Name,
                            Category = selectedItem.Category,
                            Price = selectedItem.Price,
                            Quantity = 1,
                        });
                        UpdatePayments(); 
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item First!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CartItemsDataGrid.SelectedItem != null)
            {
                MenuItem selectedItem = (MenuItem)CartItemsDataGrid.SelectedItem;
                CartItems.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select an item to remove from the cart.");
                if (CartItemsDataGrid.SelectedItem != null)
                {
                    MenuItem selectedItem = (MenuItem)CartItemsDataGrid.SelectedItem;
                    CartItems.Remove(selectedItem);
                }
                
            }

        }


        private decimal CalculateSubtotal()
        {
            decimal subtotal = CartItems.Sum(item => item.Price * item.Quantity);
            return subtotal;
        }

        private decimal CalculateTax(decimal subtotal)
        {
            decimal tax = subtotal * CanadianTaxRate;
            return tax;
        }

        
        private void UpdatePayments()
        {
            decimal subtotal = CalculateSubtotal();
            decimal tax = CalculateTax(subtotal);
            decimal total = subtotal + tax;

            subTotal.Text = subtotal.ToString("C");
            Tax.Text = tax.ToString("C");
            Total.Text = total.ToString("C");
        }
        private void UpdatePaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePayments();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            subTotal.Text = "$0.00";
            Tax.Text = "$0.00";
            Total.Text = "$0.00";

            CartItems.Clear();

       
            CartItemsDataGrid.ItemsSource = null;
            CartItemsDataGrid.ItemsSource = CartItems;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           
            MessageBox.Show($" Thank you! ");
        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string url = "https://www.centennialcollege.ca/";

               
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };

                
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
    
}


