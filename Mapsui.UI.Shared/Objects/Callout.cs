﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Interop;
using Android.Bluetooth;
using Mapsui.Extensions;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.UI.Extensions;
using Mapsui.UI.Objects;
using Mapsui.UI.Utils;
using NetTopologySuite.GeometriesGraph;
using CalloutStyle = Mapsui.Styles.CalloutStyle;
using Color = Mapsui.Styles.Color;

namespace Mapsui.UI.Objects
{
    public class Callout : BindableObject, IFeatureProvider, IDisposable, ICallout, IPropertiesInternal
    {
#if __ANDROID__ || __IOS__ || __ETO_FORMS__
        Dictionary<object,object> IPropertiesInternal.Properties { get; } = new();
#endif
        
        private readonly Pin _pin;

        public event EventHandler<CalloutClickedEventArgs>? CalloutClicked;

        public static double DefaultTitleFontSize = BluetoothClass.Device.GetNamedSize(NamedSize.Title, typeof(Label));
        public static FontAttributes DefaultTitleFontAttributes = FontAttributes.Bold;
        public static TextAlignment DefaultTitleTextAlignment = TextAlignment.Center;
        public static Color DefaultTitleFontColor = KnownColor.Black;
        public static double DefaultSubtitleFontSize = BluetoothClass.Device.GetNamedSize(NamedSize.Subtitle, typeof(Label));
        public static FontAttributes DefaultSubtitleFontAttributes = FontAttributes.None;
        public static Color DefaultSubtitleFontColor = KnownColor.Black;
        public static TextAlignment DefaultSubtitleTextAlignment = TextAlignment.Start; // Center;
#if __FORMS__
        public static string DefaultTitleFontName = Xamarin.Forms.Font.Default.FontFamily;
        public static string DefaultSubtitleFontName = Xamarin.Forms.Font.Default.FontFamily;        
#else
        public static string? DefaultTitleFontName = null; // TODO: default font per platform
        public static string? DefaultSubtitleFontName = null; // TODO: default font per platform
#endif

        #region Bindings

        /// <summary>
        /// Bindable property for the <see cref="Type"/> property
        /// </summary>
        public static readonly BindableProperty TypeProperty = BindableHelper.Create(nameof(Type), typeof(CalloutType), typeof(Callout), default(CalloutType));

        /// <summary>
        /// Bindable property for the <see cref="Anchor"/> property
        /// </summary>
        public static readonly BindableProperty AnchorProperty = BindableHelper.Create(nameof(Anchor), typeof(Point), typeof(Callout), default(Point));

        /// <summary>
        /// Bindable property for the <see cref="ArrowAlignment"/> property
        /// </summary>
        public static readonly BindableProperty ArrowAlignmentProperty = BindableHelper.Create(nameof(ArrowAlignment), typeof(ArrowAlignment), typeof(Callout), default(ArrowAlignment), defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Bindable property for the <see cref="ArrowWidth"/> property
        /// </summary>
        public static readonly BindableProperty ArrowWidthProperty = BindableHelper.Create(nameof(ArrowWidth), typeof(double), typeof(Callout), 12.0);

        /// <summary>
        /// Bindable property for the <see cref="ArrowHeight"/> property
        /// </summary>
        public static readonly BindableProperty ArrowHeightProperty = BindableHelper.Create(nameof(ArrowHeight), typeof(double), typeof(Callout), 16.0);

        /// <summary>
        /// Bindable property for the <see cref="ArrowPosition"/> property
        /// </summary>
        public static readonly BindableProperty ArrowPositionProperty = BindableHelper.Create(nameof(ArrowPosition), typeof(double), typeof(Callout), 0.5);

        /// <summary>
        /// Bindable property for the <see cref="Color"/> property
        /// </summary>
        public static readonly BindableProperty ColorProperty = BindableHelper.Create(nameof(Color), typeof(Color), typeof(Callout), KnownColor.White);

        /// <summary>
        /// Bindable property for the <see cref="BackgroundColor"/> property
        /// </summary>
        public static readonly BindableProperty BackgroundColorProperty = BindableHelper.Create(nameof(BackgroundColor), typeof(Color), typeof(Callout), KnownColor.White);

        /// <summary>
        /// Bindable property for the <see cref="ShadowWidth"/> property
        /// </summary>
        public static readonly BindableProperty ShadowWidthProperty = BindableHelper.Create(nameof(ShadowWidth), typeof(double), typeof(Callout), default(double));

        /// <summary>
        /// Bindable property for the <see cref="StrokeWidth"/> property
        /// </summary>
        public static readonly BindableProperty StrokeWidthProperty = BindableHelper.Create(nameof(StrokeWidth), typeof(double), typeof(Callout), default(double));

        /// <summary>
        /// Bindable property for the <see cref="Rotation"/> property
        /// </summary>
        public static readonly BindableProperty RotationProperty = BindableHelper.Create(nameof(Rotation), typeof(double), typeof(Callout), default(double));

        /// <summary>
        /// Bindable property for the <see cref="RotateWithMap"/> property
        /// </summary>
        public static readonly BindableProperty RotateWithMapProperty = BindableHelper.Create(nameof(RotateWithMap), typeof(bool), typeof(Callout), false);

        /// <summary>
        /// Bindable property for the <see cref="RectRadius"/> property
        /// </summary>
        public static readonly BindableProperty RectRadiusProperty = BindableHelper.Create(nameof(RectRadius), typeof(double), typeof(Callout), default(double));

        /// <summary>
        /// Bindable property for the <see cref="Padding"/> property
        /// </summary>
        public static readonly BindableProperty PaddingProperty = BindableHelper.Create(nameof(Padding), typeof(Thickness), typeof(Callout), new Thickness(6));

        /// <summary>
        /// Bindable property for the <see cref="Spacing"/> property
        /// </summary>
        public static readonly BindableProperty SpacingProperty = BindableHelper.Create(nameof(Spacing), typeof(double), typeof(Callout), 2.0);

        /// <summary>
        /// Bindable property for the <see cref="MaxWidth"/> property
        /// </summary>
        public static readonly BindableProperty MaxWidthProperty = BindableHelper.Create(nameof(MaxWidth), typeof(double), typeof(Callout), 300.0);

        /// <summary>
        /// Bindable property for the <see cref="IsClosableByClick"/> property
        /// </summary>
        public static readonly BindableProperty IsClosableByClickProperty = BindableHelper.Create(nameof(IsClosableByClick), typeof(bool), typeof(Callout), true);

        /// <summary>
        /// Bindable property for the <see cref="Content"/> property
        /// </summary>
        public static readonly BindableProperty ContentProperty = BindableHelper.Create(nameof(Content), typeof(int), typeof(Callout), -1);

        /// <summary>
        /// Bindable property for the <see cref="Title"/> property
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableHelper.Create(nameof(Title), typeof(string), typeof(Callout));

        /// <summary>
        /// Bindable property for the <see cref="TitleFontName"/> property
        /// </summary>
        public static readonly BindableProperty TitleFontNameProperty = BindableHelper.Create(nameof(TitleFontName), typeof(string), typeof(Callout), DefaultTitleFontName);

        /// <summary>
        /// Bindable property for the <see cref="TitleFontSize"/> property
        /// </summary>
        public static readonly BindableProperty TitleFontSizeProperty = BindableHelper.Create(nameof(TitleFontSize), typeof(double), typeof(Callout), DefaultTitleFontSize);

        /// <summary>
        /// Bindable property for the <see cref="TitleFontAttributes"/> property
        /// </summary>
        public static readonly BindableProperty TitleFontAttributesProperty = BindableHelper.Create(nameof(TitleFontAttributes), typeof(FontAttributes), typeof(Callout), DefaultTitleFontAttributes);

        /// <summary>
        /// Bindable property for the <see cref="TitleFontColor"/> property
        /// </summary>
        public static readonly BindableProperty TitleFontColorProperty = BindableHelper.Create(nameof(TitleFontColor), typeof(Color), typeof(Callout), DefaultTitleFontColor);

        /// <summary>
        /// Bindable property for the <see cref="TitleTextAlignment"/> property
        /// </summary>
        public static readonly BindableProperty TitleTextAlignmentProperty = BindableHelper.Create(nameof(TitleTextAlignment), typeof(TextAlignment), typeof(Callout), DefaultTitleTextAlignment);

        /// <summary>
        /// Bindable property for the <see cref="Subtitle"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleProperty = BindableHelper.Create(nameof(Subtitle), typeof(string), typeof(Callout));

        /// <summary>
        /// Bindable property for the <see cref="SubtitleFontName"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleFontNameProperty = BindableHelper.Create(nameof(SubtitleFontName), typeof(string), typeof(Callout), DefaultSubtitleFontName);

        /// <summary>
        /// Bindable property for the <see cref="SubtitleFontSize"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleFontSizeProperty = BindableHelper.Create(nameof(SubtitleFontSize), typeof(double), typeof(Callout), DefaultSubtitleFontSize);

        /// <summary>
        /// Bindable property for the <see cref="SubtitleFontAttributes"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleFontAttributesProperty = BindableHelper.Create(nameof(SubtitleFontAttributes), typeof(FontAttributes), typeof(Callout), DefaultSubtitleFontAttributes);

        /// <summary>
        /// Bindable property for the <see cref="SubtitleFontColor"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleFontColorProperty = BindableHelper.Create(nameof(SubtitleFontColor), typeof(Color), typeof(Callout), DefaultSubtitleFontColor);

        /// <summary>
        /// Bindable property for the <see cref="SubtitleTextAlignment"/> property
        /// </summary>
        public static readonly BindableProperty SubtitleTextAlignmentProperty = BindableHelper.Create(nameof(SubtitleTextAlignment), typeof(TextAlignment), typeof(Callout), DefaultSubtitleTextAlignment);

        #endregion

        public Callout(Pin pin)
        {
            _pin = pin ?? throw new ArgumentNullException(nameof(pin), "Pin shouldn't be null");
            if (_pin.Feature != null)
                Feature = _pin.Feature.Copy();
            else
                Feature = new GeometryFeature();
            Feature.Styles.Clear();
        }

        /// <summary>
        /// Pin to which this callout belongs
        /// </summary>
        public Pin Pin => _pin;

        /// <summary>
        /// Type of Callout
        /// </summary>
        /// <remarks>
        /// Could be single, detail or custom. The last is a bitmap id for an owner drawn image.
        /// </remarks>
        public CalloutType Type
        {
            get => (CalloutType)this.GetValue(TypeProperty);
            set => this.SetValue(TypeProperty, value);
        }

        /// <summary>
        /// Anchor position of Callout
        /// </summary>
        public Point Anchor
        {
            get => (Point)this.GetValue(AnchorProperty);
            set => this.SetValue(AnchorProperty, value);
        }

        /// <summary>
        /// Arrow alignment of Callout
        /// </summary>
        public ArrowAlignment ArrowAlignment
        {
            get => (ArrowAlignment)this.GetValue(ArrowAlignmentProperty);
            set => this.SetValue(ArrowAlignmentProperty, value);
        }

        /// <summary>
        /// Width from arrow of Callout
        /// </summary>
        public double ArrowWidth
        {
            get => (double)this.GetValue(ArrowWidthProperty);
            set => this.SetValue(ArrowWidthProperty, value);
        }

        /// <summary>
        /// Height from arrow of Callout
        /// </summary>
        public double ArrowHeight
        {
            get => (double)this.GetValue(ArrowHeightProperty);
            set => this.SetValue(ArrowHeightProperty, value);
        }

        /// <summary>
        /// Relative position of anchor of Callout on the side given by <see cref="ArrowAlignment"/>
        /// </summary>
        public double ArrowPosition
        {
            get => (double)this.GetValue(ArrowPositionProperty);
            set => this.SetValue(ArrowPositionProperty, value);
        }

        /// <summary>
        /// Color of stroke around Callout
        /// </summary>
        public Color Color
        {
            get => (Color)this.GetValue(ColorProperty);
            set => this.SetValue(ColorProperty, value);
        }

        /// <summary>
        /// BackgroundColor of Callout
        /// </summary>
        public Color BackgroundColor
        {
            get => (Color)this.GetValue(BackgroundColorProperty);
            set => this.SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// Shadow width around Callout
        /// </summary>
        public double ShadowWidth
        {
            get => (double)this.GetValue(ShadowWidthProperty);
            set => this.SetValue(ShadowWidthProperty, value);
        }

        /// <summary>
        /// Stroke width of frame around Callout
        /// </summary>
        public double StrokeWidth
        {
            get => (double)this.GetValue(StrokeWidthProperty);
            set => this.SetValue(StrokeWidthProperty, value);
        }

        /// <summary>
        /// Rotation of Callout around the anchor
        /// </summary>
        public double Rotation
        {
            get => (double)this.GetValue(RotationProperty);
            set => this.SetValue(RotationProperty, value);
        }

        /// <summary>
        /// Rotate Callout with map
        /// </summary>
        public bool RotateWithMap
        {
            get => (bool)this.GetValue(RotateWithMapProperty);
            set => this.SetValue(RotateWithMapProperty, value);
        }

        /// <summary>
        /// Radius of rounded corners of Callout
        /// </summary>
        public double RectRadius
        {
            get => (double)this.GetValue(RectRadiusProperty);
            set => this.SetValue(RectRadiusProperty, value);
        }

        /// <summary>
        /// Padding around content of Callout
        /// </summary>
        public Thickness Padding
        {
            get => (Thickness)this.GetValue(PaddingProperty);
            set => this.SetValue(PaddingProperty, value);
        }

        /// <summary>
        /// Space between Title and Subtitle of Callout
        /// </summary>
        public double Spacing
        {
            get => (double)this.GetValue(SpacingProperty);
            set => this.SetValue(SpacingProperty, value);
        }

        /// <summary>
        /// MaxWidth for Title and Subtitle of Callout
        /// </summary>
        public double MaxWidth
        {
            get => (double)this.GetValue(MaxWidthProperty);
            set => this.SetValue(MaxWidthProperty, value);
        }

        /// <summary>
        /// Is Callout visible on map
        /// </summary>
        public bool IsVisible => _pin.IsCalloutVisible();

        /// <summary>
        /// Is Callout closable by a click on the callout
        /// </summary>
        public bool IsClosableByClick
        {
            get => (bool)this.GetValue(IsClosableByClickProperty);
            set => this.SetValue(IsClosableByClickProperty, value);
        }

        /// <summary>
        /// Content of Callout
        /// </summary>
        public int Content
        {
            get => (int)this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Content of Callout title label
        /// </summary>
        public string Title
        {
            get => (string)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Font name to use rendering title
        /// </summary>
        public string TitleFontName
        {
            get => (string)this.GetValue(TitleFontNameProperty);
            set =>this. SetValue(TitleFontNameProperty, value);
        }

        /// <summary>
        /// Font size to rendering title
        /// </summary>
        public double TitleFontSize
        {
            get => (double)this.GetValue(TitleFontSizeProperty);
            set => this.SetValue(TitleFontSizeProperty, value);
        }

        /// <summary>
        /// Font attributes to render title
        /// </summary>
        public FontAttributes TitleFontAttributes
        {
            get => (FontAttributes)this.GetValue(TitleFontAttributesProperty);
            set => this.SetValue(TitleFontAttributesProperty, value);
        }

        /// <summary>
        /// Font color to render title
        /// </summary>
        public Color TitleFontColor
        {
            get => (Color)this.GetValue(TitleFontColorProperty);
            set => this.SetValue(TitleFontColorProperty, value);
        }

        /// <summary>
        /// Text alignment of title
        /// </summary>
        public TextAlignment TitleTextAlignment
        {
            get => (TextAlignment)this.GetValue(TitleTextAlignmentProperty);
            set => this.SetValue(TitleTextAlignmentProperty, value);
        }

        /// <summary>
        /// Content of Callout detail label
        /// </summary>
        public string Subtitle
        {
            get => (string)this.GetValue(SubtitleProperty);
            set => this.SetValue(SubtitleProperty, value);
        }

        /// <summary>
        /// Font name to use rendering subtitle
        /// </summary>
        public string SubtitleFontName
        {
            get => (string)this.GetValue(SubtitleFontNameProperty);
            set => this.SetValue(SubtitleFontNameProperty, value);
        }

        /// <summary>
        /// Font size to rendering subtitle
        /// </summary>
        public double SubtitleFontSize
        {
            get => (double)this.GetValue(SubtitleFontSizeProperty);
            set => this.SetValue(SubtitleFontSizeProperty, value);
        }

        /// <summary>
        /// Font attributes to render subtitle
        /// </summary>
        public FontAttributes SubtitleFontAttributes
        {
            get => (FontAttributes)this.GetValue(SubtitleFontAttributesProperty);
            set => this.SetValue(SubtitleFontAttributesProperty, value);
        }

        /// <summary>
        /// Font color to render subtitle
        /// </summary>
        public Color SubtitleFontColor
        {
            get => (Color)this.GetValue(SubtitleFontColorProperty);
            set => this.SetValue(SubtitleFontColorProperty, value);
        }

        /// <summary>
        /// Text alignment of title
        /// </summary>
        public TextAlignment SubtitleTextAlignment
        {
            get => (TextAlignment)this.GetValue(SubtitleTextAlignmentProperty);
            set => this.SetValue(SubtitleTextAlignmentProperty, value);
        }

        /// <summary>
        /// Feature, which belongs to callout. Should be the same as for the pin to which this callout belongs.
        /// </summary>
        public GeometryFeature Feature { get; }



        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null)
                return;

            base.OnPropertyChanged(propertyName);

            if (Type != CalloutType.Custom && propertyName.Equals(nameof(Content)))
                Type = CalloutType.Custom;

            if (IsVisible && (propertyName.Equals(nameof(Title))
                || propertyName.Equals(nameof(Subtitle))
                || propertyName.Equals(nameof(Content))
                || propertyName.Equals(nameof(Type))
                || propertyName.Equals(nameof(TitleFontName))
                || propertyName.Equals(nameof(TitleFontSize))
                || propertyName.Equals(nameof(TitleFontAttributes))
                || propertyName.Equals(nameof(TitleFontColor))
                || propertyName.Equals(nameof(TitleTextAlignment))
                || propertyName.Equals(nameof(SubtitleFontName))
                || propertyName.Equals(nameof(SubtitleFontSize))
                || propertyName.Equals(nameof(SubtitleFontAttributes))
                || propertyName.Equals(nameof(SubtitleFontColor))
                || propertyName.Equals(nameof(SubtitleTextAlignment))
                || propertyName.Equals(nameof(Spacing))
                || propertyName.Equals(nameof(MaxWidth)))
                )
                UpdateContent();
            else if (IsVisible && propertyName.Equals(nameof(ArrowAlignment))
                || propertyName.Equals(nameof(ArrowWidth))
                || propertyName.Equals(nameof(ArrowHeight))
                || propertyName.Equals(nameof(ArrowPosition))
                || propertyName.Equals(nameof(Anchor))
                || propertyName.Equals(nameof(IsVisible))
                || propertyName.Equals(nameof(Padding))
                || propertyName.Equals(nameof(Color))
                || propertyName.Equals(nameof(BackgroundColor))
                || propertyName.Equals(nameof(RectRadius)))
                UpdateCalloutStyle();

            _pin.MapView?.Refresh();
        }

        /// <summary>
        /// Callout is touched
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">CalloutClickedEventArgs</param>
        internal void HandleCalloutClicked(object? sender, CalloutClickedEventArgs e)
        {
            CalloutClicked?.Invoke(this, e);

            if (e.Handled)
                return;

            // Check, if callout is closeable by click
            if (IsClosableByClick)
            {
                _pin.HideCallout();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Checks type of Callout and activates correct content
        /// </summary>
        private void UpdateContent()
        {
            var style = Feature.Styles.Where((s) => s is CalloutStyle).FirstOrDefault() as CalloutStyle;

            if (style is null)
            {
                style = new CalloutStyle();
                Feature.Styles.Add(style);
            }

            style.Type = Type;
            style.Content = Content;
            style.Title = Title;
            style.TitleFont.FontFamily = TitleFontName;
            style.TitleFont.Size = TitleFontSize;
            style.TitleFont.Italic = (TitleFontAttributes & FontAttributes.Italic) != 0;
            style.TitleFont.Bold = (TitleFontAttributes & FontAttributes.Bold) != 0;
            style.TitleFontColor = TitleFontColor.ToMapsui();
            style.TitleTextAlignment = TitleTextAlignment.ToMapsui();
            style.Subtitle = Subtitle;
            style.SubtitleFont.FontFamily = SubtitleFontName;
            style.SubtitleFont.Size = SubtitleFontSize;
            style.SubtitleFont.Italic = (SubtitleFontAttributes & FontAttributes.Italic) != 0;
            style.SubtitleFont.Bold = (SubtitleFontAttributes & FontAttributes.Bold) != 0;
            style.SubtitleFontColor = SubtitleFontColor.ToMapsui();
            style.SubtitleTextAlignment = SubtitleTextAlignment.ToMapsui();
            style.Spacing = Spacing;
            style.MaxWidth = MaxWidth;
        }

        /// <summary>
        /// Update CalloutStyle of Feature
        /// </summary>
        private void UpdateCalloutStyle()
        {
            var style = Feature.Styles.FirstOrDefault(s => s is CalloutStyle) as CalloutStyle;

            if (style is null)
            {
                style = new CalloutStyle();
                Feature.Styles.Add(style);
            }

            style.ArrowAlignment = ArrowAlignment;
            style.ArrowHeight = (float)ArrowHeight;
            style.ArrowPosition = (float)ArrowPosition;
            style.BackgroundColor = BackgroundColor.ToMapsui();
            style.Color = Color.ToMapsui();
            style.SymbolOffset = new Offset(Anchor.X, Anchor.Y);
            style.SymbolOffsetRotatesWithMap = _pin.RotateWithMap;
            style.Padding = new MRect(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom);
            style.RectRadius = (float)RectRadius;
            style.RotateWithMap = RotateWithMap;
            style.Rotation = (float)Rotation;
            style.ShadowWidth = (float)ShadowWidth;
            style.StrokeWidth = (float)StrokeWidth;
            style.Content = Content;
        }

        /// <summary>
        /// Update content and style of callout before display it the first time
        /// </summary>
        internal void Update()
        {
            UpdateContent();
            UpdateCalloutStyle();
        }

        public virtual void Dispose()
        {
            Feature.Dispose();
        }
    }
}
