using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;

namespace AsyncIO
{
    public static class Tasks
    {


        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the synchronous way and can be used to compare the performace of sync \ async approaches. 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContent(this IEnumerable<Uri> uris)
        {
            var webClient = new WebClient();
            return uris.Select(url => webClient.DownloadString(url));
        }



        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the asynchronous way and can be used to compare the performace of sync \ async approaches. 
        /// 
        /// maxConcurrentStreams parameter should control the maximum of concurrent streams that are running at the same time (throttling). 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <param name="maxConcurrentStreams">Max count of concurrent request streams</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContentAsync(this IEnumerable<Uri> uris, int maxConcurrentStreams)
        {
            //var urisList = uris.ToList<Uri>();

            //var curentTasks = urisList.Count() > maxConcurrentStreams
            //    ? urisList
            //        .Take(maxConcurrentStreams)
            //        .Select(u => new WebClient()
            //            .DownloadStringTaskAsync(u)).ToList<Task<string>>()
            //    : urisList
            //        .Select(u => new WebClient()
            //            .DownloadStringTaskAsync(u)).ToList<Task<string>>();

            //urisList.RemoveRange(0, urisList.Count());

            //while (curentTasks.Count() > 0)
            //{
            //    var index = Task.WaitAny(curentTasks.First());

            //    yield return curentTasks.First().Result;

            //    curentTasks.RemoveAt(0);

            //    if (urisList.Count() > 0)
            //    {
            //        curentTasks.Add(new WebClient().DownloadStringTaskAsync(urisList[0]));

            //        urisList.RemoveAt(0);
            //    }
            //}
            return Enumerable.Empty<string>();
        }


        /// <summary>
        /// Calculates MD5 hash of required resource.
        /// 
        /// Method has to run asynchronous. 
        /// Resource can be any of type: http page, ftp file or local file.
        /// </summary>
        /// <param name="resource">Uri of resource</param>
        /// <returns>MD5 hash</returns>
        public async static Task<string> GetMD5Async(this Uri resource)
        {
                //return await new WebClient().DownloadDataTaskAsync(resource)
                //.ContinueWith(s => BitConverter.ToString(MD5.Create()
                //    .ComputeHash(s.Result)).Replace("-", string.Empty));
            return null;
        }

    }



}
