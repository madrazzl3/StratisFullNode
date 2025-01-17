﻿using System.Linq;
using NBitcoin;
using RocksDbSharp;

namespace Stratis.Bitcoin.Database
{
    /// <summary>A minimal RocksDb wrapper that makes it compliant with the <see cref="IDb"/> interface.</summary>
    public class RocksDb : IDb
    {
        private string dbPath;

        private RocksDbSharp.RocksDb db;

        public IDbIterator GetIterator(byte table)
        {
            return new RocksDbIterator(table, this.db.NewIterator());
        }

        public void Open(string dbPath)
        {
            this.dbPath = dbPath;
            this.db = RocksDbSharp.RocksDb.Open(new DbOptions().SetCreateIfMissing(), dbPath);
        }

        public void Clear()
        {
            this.db.Dispose();
            System.IO.Directory.Delete(this.dbPath, true);
            this.db = RocksDbSharp.RocksDb.Open(new DbOptions().SetCreateIfMissing(), this.dbPath);
        }

        public IDbBatch GetWriteBatch() => new RocksDbBatch(this.db);

        public byte[] Get(byte table, byte[] key)
        {
            return this.db.Get(new[] { table }.Concat(key).ToArray());
        }

        public void Dispose()
        {
            this.db.Dispose();
        }
    }

    /// <summary>A minimal RocksDb wrapper that makes it compliant with the <see cref="IDbBatch"/> interface.</summary>
    public class RocksDbBatch : WriteBatch, IDbBatch
    {
        private RocksDbSharp.RocksDb db;

        public RocksDbBatch(RocksDbSharp.RocksDb db)
        {
            this.db = db;
        }

        public IDbBatch Put(byte table, byte[] key, byte[] value)
        {
            return (IDbBatch)this.Put(new[] { table }.Concat(key).ToArray(), value);
        }

        public IDbBatch Delete(byte table, byte[] key)
        {
            return (IDbBatch)this.Delete(new[] { table }.Concat(key).ToArray());
        }

        public void Write()
        {
            this.db.Write(this);
        }
    }

    /// <summary>A minimal RocksDb wrapper that makes it compliant with the <see cref="IDbIterator"/> interface.</summary>
    public class RocksDbIterator : IDbIterator
    {
        private byte table;
        private Iterator iterator;

        public RocksDbIterator(byte table, Iterator iterator)
        {
            this.table = table;
            this.iterator = iterator;
        }

        public void Seek(byte[] key)
        {
            this.iterator.Seek(new[] { this.table }.Concat(key).ToArray());
        }

        public void SeekToLast()
        {
            if (this.table != 255)
            {
                // First seek past the last record in the table by attempting to seek to the start of the next table (if any).
                this.iterator.Seek(new[] { (byte)(this.table + 1) });

                // If we managed to seek to the start of the next table then go back one record to arrive at the last record of 'table'.
                if (this.iterator.Valid())
                {
                    this.iterator.Prev();
                    return;
                }
            }

            // If there is no next table then simply seek to the last record in the db as that will be the last record of 'table'.
            this.iterator.SeekToLast();
        }

        public void Next()
        {
            this.iterator.Next();
        }

        public void Prev()
        {
            this.iterator.Prev();
        }

        public bool IsValid()
        {
            return this.iterator.Valid() && this.iterator.Value()[0] == this.table;
        }

        public byte[] Key()
        {
            return this.iterator.Key().Skip(1).ToArray();
        }

        public byte[] Value()
        {
            return this.iterator.Value();
        }

        public void Dispose()
        {
            this.iterator.Dispose();
        }
    }
}
