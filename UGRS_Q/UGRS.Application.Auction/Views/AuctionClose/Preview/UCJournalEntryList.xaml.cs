using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using UGRS.Core.Auctions.Entities.Financials;

namespace UGRS.Application.Auctions.AuctionClose
{
    /// <summary>
    /// Interaction logic for UCJournalEntryList.xaml
    /// </summary>
    public partial class UCJournalEntryList : UserControl
    {
        IList<JournalEntry> mLstJornalEntry;
        public UCJournalEntryList(IList<JournalEntry> pLstJournalEntry)
        {
            InitializeComponent();
            mLstJornalEntry = pLstJournalEntry;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<JournalEntry> lLstJournalEntry = mLstJornalEntry.ToList();
            dgJounrnalEntry.ItemsSource = lLstJournalEntry[0].Lines.ToList();
        }

    }
}
