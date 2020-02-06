using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM
{
    class ViewModel : ViewModelBase
    {
        string[] MemoryTab = { "𒉼", "𒉼", "☉", "☉", "㐃", "㐃" };
        int NumberOfOpenCards;
        string LastCard;
        int LastIndex;
        bool canClick = true;
        bool newGame = false;
        int FoundPairs;

        public ICommand Click { private set; get; }
        public ICommand NewGameCommand { private set; get; }

        public ObservableCollection<bool> isEnabled;
        public ObservableCollection<string> content;

        public ViewModel()
        {
            NewGame();


            Click = new RelayCommand(
                (parameter) => 
                {
                    int param = Convert.ToInt32(parameter);

                    if (isEnabled[param]) 
                    {
                        Content[param] = MemoryTab[param];

                        isEnabled[param] = false;

                        NumberOfOpenCards++;

                        if (NumberOfOpenCards == 1)
                        {
                            LastCard = MemoryTab[param];
                            LastIndex = param; 
                        }
                        else if (NumberOfOpenCards == 2)
                        {
                            if (!Compare(LastCard, MemoryTab[param]))
                            {
                                canClick = false;
                                WaitAndReset(param, LastIndex); 
                            }
                            else 
                                FoundPairs++;


                            NumberOfOpenCards = 0;
                        }
                    }

                    if (FoundPairs == 3) 
                        newGame = true;

                    RefreshCanExecutes();
                },
                (parameter) =>
                {
                    return canClick;
                });


            NewGameCommand = new RelayCommand(
                (obj) =>
                {
                    NewGame(); 
                },
                (obj) =>
                {
                    return newGame;
                });
        }

        private bool Compare(string a, string b)
        {
            return a.Equals(b);
        }

        private void NewGame() 
        {
            Content = new ObservableCollection<string>(new string[9]); 
            IsEndabled = new ObservableCollection<bool>(new bool[] { true, true, true, true, true, true, true, true, true });
            Shuffle(MemoryTab);  
            NumberOfOpenCards = 0;
            FoundPairs = 0;
        }


        private void Shuffle(string[] items) 
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            Random rand = new Random();

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                string temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        private async Task WaitAndReset(int i, int j)
        {
            await Task.Delay(2000);
            Content[i] = "";
            Content[j] = "";
            IsEndabled[i] = true;
            IsEndabled[j] = true;

            canClick = true; 
        }


        private void RefreshCanExecutes()
        {
            ((RelayCommand)Click).OnCanExecuteChanged();
        }

        public ObservableCollection<string> Content
        {
            private set { SetProperty(ref content, value); }
            get { return content; }
        }

        public ObservableCollection<bool> IsEndabled
        {
            private set { SetProperty(ref isEnabled, value); }
            get { return isEnabled; }
        }
    }
}
