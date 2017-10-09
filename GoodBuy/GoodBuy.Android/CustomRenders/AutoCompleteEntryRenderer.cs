using System.Collections.Generic;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using GoodBuy.Core.Controls;
using Xamarin.Forms;
using goodBuy.Droid.CustomRenders;
using Android.Views;
using Android.Views.InputMethods;
using Android.Text;
using Java.Lang;
using System;
using Android.Util;
using Android.Text.Method;
using Android.Content.Res;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(AutoComplete), typeof(AutoCompleteEntryRenderer))]
namespace goodBuy.Droid.CustomRenders
{
    public class AutoCompleteEntryRenderer : ViewRenderer<AutoComplete, AutoCompleteTextView>, ITextWatcher, TextView.IOnEditorActionListener
    {
        public AutoCompleteEntryRenderer()
        {
            AutoPackage = false;
            GoodBuy.Service.OfertasService.OnCollectionLoaded += UpdateAdapterLoad;
        }
        public string Name { get; set; }
        protected override void OnElementChanged(ElementChangedEventArgs<AutoComplete> e)
        {
            base.OnElementChanged(e);
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                var textView = CreateNativeControl();
                textView.ImeOptions = ImeAction.Done;
                textView.AddTextChangedListener(this);
                textView.SetOnEditorActionListener(this);
                //textView.OnKeyboardBackPressed += OnKeyboardBackPressed;
                SetNativeControl(textView);
            }

            Control.Hint = Element.Placeholder;
            Control.Text = Element.Text;
            UpdateInputType();

            UpdateColor();
            UpdateAlignment();
            UpdateFont();
            UpdatePlaceholderColor();

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var control = new AutoCompleteTextView(Xamarin.Forms.Forms.Context);

                    if (!string.IsNullOrEmpty(e.NewElement.Placeholder))
                        control.Hint = e.NewElement.Placeholder;

                    SetNativeControl(control);
                }
                Name = e.NewElement.Name;
                UpdateAdapter(e.NewElement.ItemsSource);
            }
        }
        ColorStateList _hintTextColorDefault;
        ColorStateList _textColorDefault;
        bool _disposed;


        bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            // Fire Completed and dismiss keyboard for hardware / physical keyboards
            if (actionId == ImeAction.Done || (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter))
            {
                Control.ClearFocus();
                //v.HideKeyboard();
                ((IEntryController)Element).SendCompleted();
            }

            return true;
        }

        void ITextWatcher.AfterTextChanged(IEditable s)
        {
        }

        void ITextWatcher.BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        void ITextWatcher.OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (string.IsNullOrEmpty(Element.Text) && s.Length() == 0)
                return;

            ((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                if (Control != null)
                {
                    //Control.OnKeyboardBackPressed -= OnKeyboardBackPressed;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
                Control.Hint = Element.Placeholder;
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                UpdateInputType();
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
            {
                if (Control.Text != Element.Text)
                {
                    Control.Text = Element.Text;
                    if (Control.IsFocused)
                    {
                        Control.SetSelection(Control.Text.Length);
                        Control.ShowDropDown();
                    }
                }
            }
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                UpdateColor();
            else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
                UpdateInputType();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
                UpdatePlaceholderColor();

            base.OnElementPropertyChanged(sender, e);
        }

        protected virtual NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
        {
            // Override this in a custom renderer to use a different NumberKeyListener 
            // or to filter out input types you don't want to allow 
            // (e.g., inputTypes &= ~InputTypes.NumberFlagSigned to disallow the sign)
            //return LocalizedDigitsKeyListener.Create(inputTypes);
            return null;
        }

        void UpdateAlignment()
        {
            //Control.Gravity = Element.HorizontalTextAlignment.ToHorizontalGravityFlags();
        }

        void UpdateColor()
        {
            if (Element.TextColor.IsDefault)
            {
                if (_textColorDefault == null)
                {
                    // This control has always had the default colors; nothing to update
                    return;
                }

                // This control is being set back to the default colors
                Control.SetTextColor(_textColorDefault);
            }
            else
            {
                if (_textColorDefault == null)
                {
                    // Keep track of the default colors so we can return to them later
                    // and so we can preserve the default disabled color
                    _textColorDefault = Control.TextColors;
                }

                Control.SetTextColor(Element.TextColor.ToAndroidPreserveDisabled(_textColorDefault));
            }
        }

        void UpdateFont()
        {
            //Control.Typeface = Element.ToTypeface();
            Control.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
        }

        void UpdateInputType()
        {
            Entry model = Element;
            var keyboard = model.Keyboard;

            Control.InputType = keyboard.ToInputType();

            if (keyboard == Keyboard.Numeric)
            {
                Control.KeyListener = GetDigitsKeyListener(Control.InputType);
            }

            if (model.IsPassword && ((Control.InputType & InputTypes.ClassText) == InputTypes.ClassText))
                Control.InputType = Control.InputType | InputTypes.TextVariationPassword;
            if (model.IsPassword && ((Control.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber))
                Control.InputType = Control.InputType | InputTypes.NumberVariationPassword;
        }

        protected override AutoCompleteTextView CreateNativeControl()
        {
            return new AutoCompleteTextView(Context);
        }
        void UpdatePlaceholderColor()
        {
            Color placeholderColor = Element.PlaceholderColor;

            if (placeholderColor.IsDefault)
            {
                if (_hintTextColorDefault == null)
                {
                    // This control has always had the default colors; nothing to update
                    return;
                }

                // This control is being set back to the default colors
                Control.SetHintTextColor(_hintTextColorDefault);
            }
            else
            {
                if (_hintTextColorDefault == null)
                {
                    // Keep track of the default colors so we can return to them later
                    // and so we can preserve the default disabled color
                    _hintTextColorDefault = Control.HintTextColors;
                }

                Control.SetHintTextColor(placeholderColor.ToAndroidPreserveDisabled(_hintTextColorDefault));
            }
        }

        void OnKeyboardBackPressed(object sender, EventArgs eventArgs)
        {
            Control?.ClearFocus();
        }
        private void UpdateAdapterLoad(string name, List<string> items)
        {
            if (name == Name)
                UpdateAdapter(items);
        }

        private void UpdateAdapter(List<string> items)
        {
            ArrayAdapter autoCompleteAdapter = new ArrayAdapter(Xamarin.Forms.Forms.Context, Android.Resource.Layout.SimpleDropDownItem1Line, items);
            Control.Adapter = autoCompleteAdapter;
        }
    }
}