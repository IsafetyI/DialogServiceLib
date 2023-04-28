using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DialogServiceLib
{
    public class CreateDialogPreset<TResult> : DialogPreset<TResult>, ICreateDialogPreset<TResult>
    {
        public CreateDialogPreset<TResult> SetItemCreationDelegate(CreateDialogItemCreationDelegate<TResult> itemCreationDelegate)
        {
            if (itemCreationDelegate == null)
                throw new Exception();
            this.itemCreationDelegate = itemCreationDelegate;
            return this;
        }

        public override CreateDialogPreset<TResult> UseView<TView>()
        {
            dialogViewType = typeof(TView);
            return this;
        }

        public override CreateDialogPreset<TResult> UseViewModel<TViewModel>()
        {
            dialogViewModelType = typeof(TViewModel);
            return this;
        }

        public Tuple<TResult?, bool> ShowDialog()
        {
            if (dialogViewType == null)
                throw new NullReferenceException("Dialog View type shouldn't be null");

            if (dialogViewModelType == null)
                throw new NullReferenceException("Dialog ViewModel type shouldn't be null");

            var viewModelConstructor = dialogViewModelType.GetConstructor(new Type[] { });
            if (viewModelConstructor == null)
                throw new NullReferenceException("Unable to find public ViewModel constructor with no parameters");
            DialogViewModelBase<TResult> dialogViewModel = (DialogViewModelBase<TResult>)viewModelConstructor.Invoke(new object[] { });

            var viewConstructor = dialogViewType.GetConstructor(new Type[] { });
            if (viewConstructor == null)
                throw new NullReferenceException("Unable to find public View constructor with no parameters");
            Window dialogView = (Window)viewConstructor.Invoke(new object[] { });

            TResult item = itemCreationDelegate.Invoke();
            if (item == null)
                throw new NullReferenceException("Item producing delegate returned null");
            dialogViewModel.InputValue = item;
            dialogViewModel.Mode = AddEditDialogMode.Add;

            dialogView.DataContext = dialogViewModel;
            dialogView.ShowDialog();
            if (dialogViewModel.DialogResult == null)
                return new Tuple<TResult?, bool>(default(TResult?), false);
            return dialogViewModel.DialogResult;
        }

        protected CreateDialogItemCreationDelegate<TResult> itemCreationDelegate = (() => {
            ConstructorInfo? constructor;
            TResult? value = default(TResult);
            if ((constructor = typeof(TResult).GetConstructor(new Type[] { })) != null)
            {
                try
                {
                    value = (TResult)constructor.Invoke(new object[] { });
                }
                catch (Exception)
                {
                    throw new Exception(typeof(TResult) + " not implements new(), so you should specify EditDialogItemCreationDelegate to create it your own way");
                }
            }
            else
                throw new Exception(typeof(TResult) + " not implements new(), so you should specify EditDialogItemCreationDelegate to create it your own way");
            return value!;
        });
    }
}
