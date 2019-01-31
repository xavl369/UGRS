using System;
using System.Windows.Input;
using UGRS.Core.Application.Utility;

namespace UGRS.Core.Application.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> mObjExecute;

        private readonly Predicate<object> mBolCanExecute;

        public RelayCommand(Action<object> pObjExecute)
            : this(pObjExecute, null)
        {
        }

        public RelayCommand(Action<object> pObjExecute, Predicate<object> pBolCanExecute)
        {
            ExceptionUtility.CheckArgumentNotNull(pObjExecute, "Execute");

            this.mObjExecute = pObjExecute;
            this.mBolCanExecute = pBolCanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object pObjParameter)
        {
            return this.mBolCanExecute == null ? true : this.mBolCanExecute(pObjParameter);
        }

        public void Execute(object pObjParameter)
        {
            this.mObjExecute(pObjParameter);
        }
    }
}
