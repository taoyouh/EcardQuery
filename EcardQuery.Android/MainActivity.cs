using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.IO;
using System.Net.Http;

namespace EcardQuery
{
    [Activity(Label = "@string/AppName", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public static EcardWebsiteHelper helper;
        ImageView randImage;
        TextView statusText;
        Button loginButton;
        ProgressBar progressBar;
        EditText idBox;
        EditText passwdBox;
        EditText randBox;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            DBCS.DBCSEncoding.Assets = this.Assets;
            var x = DBCS.DBCSEncoding.GetDBCSEncoding("gb2312");
            helper = new EcardWebsiteHelper();

            RefreshCheckPic();

            randImage = FindViewById<ImageView>(Resource.Id.Main_RandImage);
            randImage.Click += RandImage_Click;

            statusText = FindViewById<TextView>(Resource.Id.Main_StatusText);

            loginButton = FindViewById<Button>(Resource.Id.Main_LoginButton);
            loginButton.Click += LoginButton_Click;

            progressBar = FindViewById<ProgressBar>(Resource.Id.Main_ProgressBar);

            idBox = FindViewById<EditText>(Resource.Id.Main_IdBox);
            passwdBox = FindViewById<EditText>(Resource.Id.Main_PasswdBox);
            randBox = FindViewById<EditText>(Resource.Id.Main_RandBox);

            randBox.KeyPress += RandBox_KeyPress;
        }

        private async void RandBox_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter && e.Event.Action==KeyEventActions.Down)
            {
                e.Handled = true;
                await LoginAsync();
            }
            else
            {
                e.Handled = false;
            }
        }

        private void RandImage_Click(object sender, EventArgs e)
        {
            RefreshCheckPic();
        }

        private async void RefreshCheckPic()
        {
            Stream stream;
            const int MAX_RETRY = 3;
            Exception ex = null;
            for (int i = 0; i < MAX_RETRY; i++)
            {
                try
                {
                    stream = await helper.GetCheckPicAsync();
                    Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                    randBox.Measure(0, 0);
                    randImage.LayoutParameters = new LinearLayout.LayoutParams(
                        randBox.MeasuredHeight * bitmap.Width / bitmap.Height,
                        randImage.LayoutParameters.Height);
                    randImage.SetImageBitmap(bitmap);
                    ex = null;
                    break;
                }
                catch (Exception exception)
                {
                    ex = exception;
                }
            }
            if (ex != null)
            {
                randImage.SetImageDrawable(
                    GetDrawable(Android.Resource.Drawable.IcMenuGallery));
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async System.Threading.Tasks.Task LoginAsync()
        {
            statusText.Text = "";
            statusText.Visibility = ViewStates.Gone;
            loginButton.Enabled = false;
            progressBar.Visibility = ViewStates.Visible;
            try
            {
                await helper.LoginAsync(EcardWebsiteHelper.LoginType.PersonId,
                    idBox.Text, passwdBox.Text, randBox.Text);
                await helper.HistoryInquiryInitAsync();
                StartActivity(typeof(MeCenterActivity));
                Finish();
            }
            catch (ArgumentException ex)
            {
                statusText.Visibility = ViewStates.Visible;
                switch (ex.ParamName)
                {
                    case "name":
                        statusText.Text = "用户名不正确";
                        break;
                    case "passwd":
                        statusText.Text = "密码不正确";
                        break;
                    case "rand":
                        statusText.Text = "验证码不正确";
                        break;
                }
                RefreshCheckPic();
            }
            catch (InvalidOperationException)
            {
                statusText.Visibility = ViewStates.Visible;
                statusText.Text = "登录过于频繁，请10秒后再试。";
                RefreshCheckPic();
            }
            catch (HttpRequestException)
            {
                statusText.Visibility = ViewStates.Visible;
                statusText.Text = "网络连接失败，请稍后再试。";
                RefreshCheckPic();
            }
            finally
            {
                loginButton.Enabled = true;
                progressBar.Visibility = ViewStates.Gone;
            }
        }
    }
}

