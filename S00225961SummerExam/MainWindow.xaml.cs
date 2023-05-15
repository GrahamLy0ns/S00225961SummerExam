using System;
using System.Collections.Generic;
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

namespace S00225961SummerExam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //query to populate ui with database information
            MovieData db = new MovieData();
            //populating list box with movies
            var query = from a in db.Movies
                        select a.Title;
            movieListBox.ItemsSource = query.ToList();

        }

        private void movieListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //populating the movie synopsis with data from the database
            MovieData db = new MovieData();
            var selectedItem = movieListBox.SelectedItem;

            var query = from a in db.Movies
                        where a.Title == selectedItem.ToString()
                        select a.Description;
            movieDescriptionTextBox.Text = query.ToList()[0];
            
        }

        private void bookSeatsBtn_Click(object sender, RoutedEventArgs e)
        {

            //booking movie code
            MovieData db = new MovieData();
            DateTime date = new DateTime() { };
            int numberOfTickets = int.Parse(requiredSeatsTextBox.Text);
            string movie = "";
            //error handling
            try
            {
                movie = movieListBox.SelectedItem.ToString();
                date = DatePickerObject.SelectedDate.Value;
            }
            catch(Exception ex)
            {
             
                MessageBox.Show(ex.ToString() + "\n\nNo date for movie chosen or no movie selected");
            }
            //more error handling
            if (date == null)
            {
                MessageBox.Show("Error must select a date for movie booking");
            }
            else if (numberOfTickets <=0)
            {
                MessageBox.Show("Error must enter at least one ticket to complete movie booking");
            }
            else
            {
                //proceed with movie booking

                //quering database to determine if seats are < 100
                var query = from a in db.Bookings
                            where a.BookingDate == date && a.Movie.Title == movie
                            select a.NumberOfTickets;
                string currentTickets = "";
                int currentNumberOfTickets = 0;
                try
                {
                    currentTickets = query.ToList()[0].ToString();
                    currentNumberOfTickets = int.Parse(currentTickets);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("query returned null because this is the first booking for this movie or date");
                }
                

                //updating ticket availablity
                availableSeatsTextBlock.Text = currentTickets;

                if (currentNumberOfTickets + numberOfTickets <= 100 || currentTickets == "" && currentNumberOfTickets == 0)
                {
                    //write to database

                    Booking newBooking = new Booking()
                    {
                        BookingDate = date,
                        NumberOfTickets = numberOfTickets,
                        Movie = { Title = movie, Cast = "", Description = "", ImageName = ""}
                    };
                    db.Bookings.Add(newBooking);
                    db.SaveChanges();
                    MessageBox.Show("Booking Added to Database!");
                }
                else
                {
                    MessageBox.Show("Sold Out!");
                }
            }
           
        }
    }
}
