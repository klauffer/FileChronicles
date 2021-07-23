﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Events;

namespace FileChronicles
{
    /// <summary>
    /// the object that keeps accounts of events through the history of a transaction
    /// </summary>
    public class Chronicler : IAsyncDisposable
    {
        private readonly Chronicle _chronicle;

        /// <summary>
        /// Keep Chronicler from being instantiated directly
        /// </summary>
        private Chronicler() 
        {
            _chronicle = new Chronicle();
        }

        /// <summary>
        /// Begins a new Chronicler that will track the this Chonicle
        /// </summary>
        /// <returns>a new Chronicler</returns>
        public static Chronicler Begin() => new Chronicler();

        /// <summary>
        /// Commits all recorded events in the chronicle tracked by this Chronicler
        /// </summary>
        /// <returns></returns>
        public async Task<EventResult<string, ErrorCode>> Commit()
        {
            return await _chronicle.Commit();
        }

        /// <summary>
        /// forgets uncommitted events and rollsback committed events in the chronicle tracked by this Chronicler
        /// </summary>
        /// <returns></returns>
        public async Task Rollback()
        {
            await _chronicle.Rollback();
        }

        /// <summary>
        /// Creates a file at the specified location with the contents being the specified Bytes
        /// </summary>
        /// <param name="path">The location where the file will be made</param>
        /// <param name="bytes">the contents of the file</param>
        /// <param name="cancellationToken">to cancel writing the file</param>
        /// <returns></returns>
        public async Task<EventResult<string, ErrorCode>> Create(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            var createFileEvent = new CreateFileEvent(path, bytes, cancellationToken);
            return await _chronicle.AddEvent(createFileEvent);

        }

        /// <summary>
        /// Properly disposes of asynchronous resources
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_chronicle != null)
            {
                await _chronicle.Rollback();
            }
        }
    }
}
