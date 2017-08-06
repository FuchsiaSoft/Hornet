using Hornet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hornet.ViewModel.ViewModel.DatabaseManagement
{
    class AddEditHashSet : DataEntryViewModelBase<HashGroup>
    {
        #region Binding Properties

        

        #endregion

        public AddEditHashSet(HashGroup entity, DataEntryMode mode = DataEntryMode.New, Action exitAction = null) : 
            base(entity, mode, exitAction)
        {

        }

        protected override bool CanCancel(Window window)
        {
            return true; //can always cancel this one
        }

        protected override bool CanSave(Window window)
        {
            throw new NotImplementedException();
        }

        protected override Task LoadAlways()
        {
            throw new NotImplementedException();
        }

        protected override Task LoadExisting()
        {
            throw new NotImplementedException();
        }

        protected override Task LoadNew()
        {
            throw new NotImplementedException();
        }

        protected override Task SaveExisting()
        {
            throw new NotImplementedException();
        }

        protected override Task SaveNew()
        {
            throw new NotImplementedException();
        }
    }
}
