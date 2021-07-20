﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileChronicles
{
    /// <summary>
    /// the object that keeps accounts of events through the history of a transaction
    /// </summary>
    public sealed class Chronicler
    {

        /// <summary>
        /// Creates a file at the specified location with the contents being the specified Bytes
        /// </summary>
        /// <param name="path">The location where the file will be made</param>
        /// <param name="bytes">the contents of the file</param>
        /// <param name="cancellationToken">to cancel writing the file</param>
        /// <returns></returns>
        public async Task<EventResult<string, ErrorCode>> Create(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            if(!File.Exists(path))
            {
                await File.WriteAllBytesAsync(path, bytes, cancellationToken);
                return new EventResult<string, ErrorCode>.Success(path);
            }
            return new EventResult<string, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
        }
    }
}
