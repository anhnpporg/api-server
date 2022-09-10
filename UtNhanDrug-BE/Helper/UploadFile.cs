using Firebase.Auth;
using Firebase.Storage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UtNhanDrug_BE.Helper
{
    public class UploadFile
    {
        private static string ApiKey = "AIzaSyB-9fD6pq0a7yjziqoxGIdHhaZEC5m2KG8";
        private static string Bucket = "utnhandrug.appspot.com";
        private static string AuthEmail = "admin@gmail.com";
        private static string AuthPassword = "123456";

        public async void Upload(FileStream stream, string filename)
        {

            // of course you can login using other method, not just email+password
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage (
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                //.Child("images")
                //.Child("test")
                .Child(filename)
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            
            // cancel the upload
            // cancellation.Cancel();

            try
            {
                // error during upload will be thrown when you await the task
                Console.WriteLine("Download link:\n" + await task);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
            }
        }

    }
}
