using System.Windows.Controls;

namespace Client {

    public class BasePage<VM> : Page 
        where VM : BaseViewModel, new() 
{

        #region Private

        private VM mViewModel;

        #endregion

        #region Properties

        public VM ViewModel
        {
            get { return mViewModel; }
            set
            {
                // Ako se ne menja onda nista
                if (mViewModel == value) {
                    return;
                }
 
                mViewModel = value;

                // Updateujemo DataContext
                this.DataContext = mViewModel;
            }
        }

        #endregion

        #region Constuctors

        public BasePage()
        {

            // Default
            this.ViewModel = new VM();
        }

        #endregion
    }

}
