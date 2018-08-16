require('dotenv').config()
const MongoClient = require('mongodb').MongoClient

//Setting the MongoDB Cluster URI
const uri = process.env.MONGO_URI
const dbName = 'test'
const collectionName = 'inventory'
let _client;
let db;
let changeStream;

const initConnection = () => {
  return MongoClient.connect(uri).then((client) => {
    _client = client
    _db = client.db(dbName)
  })
}

const initChangeStream = () => {
  const collection = _db.collection(collectionName);
  changeStream = collection.watch();
  changeStream.on('change', (change) => { console.log(change) })
  changeStream.on('error', (e) => { console.error(e)})
  changeStream.on('close', () => { console.log('Closing Change Stream') })
  changeStream.on('end', () => { console.log('End of change stream') })
}

const close = () => {
  if (changeStream)
    changeStream.close()
  if (_client)
    _client.close()
}
process.on('SIGTERM', () => {
  console.warn('Shutting down')
  close()
});

// listen for INT signal e.g. Ctrl-C
process.on('SIGINT', () => {
  console.warn('Shutting down')
  close()
});


initConnection().then(() => initChangeStream()).catch((err) => {
  console.error(err)
})