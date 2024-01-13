using CopyingServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DialogServiceLib
{
    public class EditDialogPreset<TResult> : DialogPreset<TResult>, IEditDialogPreset<TResult>
    {
        public override EditDialogPreset<TResult> UseView<TView>()
        {
            dialogViewType = typeof(TView);
            return this;
        }

        public override EditDialogPreset<TResult> UseViewModel<TViewModel>()
        {
            dialogViewModelType = typeof(TViewModel);
            return this;
        }

        public EditDialogPreset<TResult> SetCopyingService(ICopyingService copyingService)
        {
            SetForvardCopyingService(copyingService);
            SetBackwardCopyingService(copyingService);
            return this;
        }

        public EditDialogPreset<TResult> SetForvardCopyingService(ICopyingService copyingService)
        {
            if (copyingService.GetType().GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition()).Where(i => i == typeof(ICopyingService<>)).Any()
                && !copyingService.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICopyingService<>) && i.GetGenericArguments().First() == typeof(TResult)).Any())
            {
                throw new Exception();
            }
            forvardCopyingService = copyingService;
            return this;
        }

        public EditDialogPreset<TResult> SetBackwardCopyingService(ICopyingService copyingService)
        {
            if (copyingService.GetType().GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition()).Where(i => i == typeof(ICopyingService<>)).Any()
                && !copyingService.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICopyingService<>) && i.GetGenericArguments().First() == typeof(TResult)).Any())
            {
                throw new Exception();
            }
            backwardCopyingService = copyingService;
            return this;
        }

        public EditDialogPreset<TResult> SetItemCreationDelegate(EditDialogItemCreationDelegate<TResult> itemCreationDelegate)
        {
            if (itemCreationDelegate == null)
                throw new Exception();
            this.itemCreationDelegate = itemCreationDelegate;
            return this;
        }

        public Tuple<TResult?, bool> ShowDialog(TResult value)
        {
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

                TResult item = itemCreationDelegate.Invoke(value);
                if (item == null)
                    throw new NullReferenceException("Item producing delegate returned null");

                var inputValue = forvardCopyingService.Copy(value, item);
                if (inputValue == null)
                    throw new Exception("forwardCopyingService returned null");
                if (inputValue.GetType() != typeof(TResult) && inputValue.GetType() != typeof(TResult?))
                    throw new Exception("forwardCopyingService returned value of wrong type. Expected " + typeof(TResult) + " But was " + inputValue.GetType() + ".");

                dialogViewModel.InputValue = (TResult)inputValue;
                dialogViewModel.Mode = AddEditDialogMode.Edit;

                dialogView.DataContext = dialogViewModel;
                dialogView.ShowDialog();

                if (dialogViewModel.DialogResult == null || !dialogViewModel.DialogResult.Item2)
                    return new Tuple<TResult?, bool>(default(TResult?), false);

                var outputValue = backwardCopyingService.Copy(dialogViewModel.DialogResult.Item1, value);
                if (outputValue == null)
                    throw new Exception("backwardCopyingService returned null");
                if (!outputValue.GetType().IsAssignableTo(typeof(TResult)))
                    throw new Exception("backwardCopyingService returned value of wrong type. Expected " + typeof(TResult) + " But was " + outputValue.GetType() + ".");

                return new Tuple<TResult?, bool>((TResult)outputValue, dialogViewModel.DialogResult.Item2);
            }
        }

        protected ICopyingService forvardCopyingService = new ValueCopyingService<TResult>();

        protected ICopyingService backwardCopyingService = new ValueCopyingService<TResult>();

        protected EditDialogItemCreationDelegate<TResult> itemCreationDelegate = (item => {
            ConstructorInfo? constructor;
            TResult? value = default(TResult);
            if ((constructor = typeof(TResult).GetConstructor(new Type[] { })) != null)
            {
                try
                {
                    value = (TResult?)constructor.Invoke(new object[] { });
                }
                catch (Exception)
                {
                    throw new Exception(typeof(TResult) + " not implements new(), so you should specify EditDialogItemCreationDelegate to create it your own way");
                }
            }
            else
            {
                throw new Exception(typeof(TResult) + " not implements new(), so you should specify EditDialogItemCreationDelegate to create it your own way");
            }
            return value!;
        });
    }
}
