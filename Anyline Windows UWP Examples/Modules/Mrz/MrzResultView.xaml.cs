using AnylineSDK.Modules.Mrz;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnylineExamplesApp.Modules.Mrz
{
    public sealed partial class MrzResultView : UserControl, IDisposable
    {               
        public MrzResultView()
        {
            InitializeComponent();
            FadeInStoryBoard.Completed += FadeInStoryBoard_Completed;
            FadeOutStoryBoard.Completed += FadeOutStoryBoard_Completed;

            Visibility = Visibility.Collapsed;
        }

        public void UpdateResult(Identification id)
        {
            TxtType.Text = id.DocumentType;
            TxtSurnames.Text = id.SurNames;
            TxtSex.Text = id.Sex;
            TxtNumber.Text = id.DocumentNumber;
            TxtGivenNames.Text = id.GivenNames;
            TxtExpirationDate.Text = id.ExpirationDate;
            TxtDayOfBirth.Text = id.DayOfBirth;
            TxtCountryCodeNational.Text = id.NationalityCountryCode;

            string mrzString;

            if ("P".Equals(id.DocumentType))
            {
                mrzString = string.Format("{0,-44}", string.Format("P<{0}{1}<<{2}", id.NationalityCountryCode,
                        id.SurNames, id.GivenNames));

                mrzString += Environment.NewLine;
                mrzString += Environment.NewLine;

                mrzString += string.Format("{0,-42}{1,1}{2,1}", string.Format("{0,-9}{1}{2}{3}{4}{5}{6}{7}{8}",
                                id.DocumentNumber, id.CheckDigitNumber,
                                id.IssuingCountryCode,
                                id.DayOfBirth, id.CheckDigitDayOfBirth,
                                id.Sex,
                                id.ExpirationDate, id.CheckDigitExpirationDate,
                                id.PersonalNumber),
                        id.CheckDigitPersonalNumber, id.CheckDigitFinal);
            }
            else
            {

                mrzString = string.Format("{0,-30}", string.Format("{0}{1,-3}{2,-9}{3}",
                        id.DocumentType, id.NationalityCountryCode,
                        id.DocumentNumber, id.CheckDigitNumber));

                mrzString += Environment.NewLine;


                mrzString += string.Format("{0,-29}{1,1}", string.Format("{0}{1}{2}{3}{4}{5}",
                                id.DayOfBirth, id.CheckDigitDayOfBirth,
                                id.Sex, id.ExpirationDate,
                                id.CheckDigitExpirationDate, id.IssuingCountryCode),
                        id.CheckDigitFinal);

                mrzString += Environment.NewLine;

                mrzString += string.Format("{0,-30}", string.Format("{0}<<{1}",
                        id.SurNames, id.GivenNames));
            }
            mrzString = mrzString.Replace(" ", "<");

            TxtMrzResultText.Text = mrzString;
        }

        public void FadeIn()
        {
            Visibility = Visibility.Visible;
            FadeOutStoryBoard.Stop();
            FadeInStoryBoard.Begin();
        }

        public void FadeOut()
        {
            FadeInStoryBoard.Stop();
            FadeOutStoryBoard.Begin();            
        }
        
        private void FadeOutStoryBoard_Completed(object sender, object e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void FadeInStoryBoard_Completed(object sender, object e)
        {
        }

        public void Dispose()
        {
            FadeInStoryBoard.Completed -= FadeInStoryBoard_Completed;
            FadeOutStoryBoard.Completed -= FadeOutStoryBoard_Completed;
        }        
    }
}
