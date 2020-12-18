'use strict';

const AWS = require('aws-sdk');
AWS.config.loadFromPath('./config.json');
const dynamoDb = new AWS.DynamoDB.DocumentClient();

module.exports.putItems = async event => {
  const requestBody = JSON.parse(event.body);
  const items = requestBody.items;
  return await putItems(putItemParams(items));
};

const putItemParams = (items) => {
  // return {
  //   TableName: process.env.ITEMS_TABLE,
  //   Item: {
  //     roomId: roomId,
  //     itemId: itemId,
  //     position: position,
  //     rotation: rotation
  //   }
  // };

  let itemRequests = [];
  items.map(item => {
    itemRequests.push({
      PutRequest: {
        Item: item
      }
    })
  });

  return {
    RequestItems: {
      [process.env.ITEMS_TABLE]: itemRequests
    }
  }
}

const putItems = params => {
  return new Promise((resolve, reject) => {
    dynamoDb.batchWrite(params, function(err, data) {
      if (err) {
        reject({statusCode: 500, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(err)});
      } else {
        resolve({statusCode: 200, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(data)});
      }
    });


    // items.map(item => {
    //   //let params = putItemParams(item.roomId, item.itemId, item.position, item.rotation);

    //   dynamoDb.batchWriteItem(params, function(err, data) {
    //     if (err) {
    //       reject({statusCode: 500, headers: {
    //         "Access-Control-Allow-Origin": "*",
    //         "Access-Control-Allow-Credentials": true,
    //         "Access-Control-Allow-Headers": "Content-Type",
    //         "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
    //       }, body: JSON.stringify(err)});
    //     }
    //   });
    // });

    // resolve({statusCode: 200, headers: {
    //   "Access-Control-Allow-Origin": "*",
    //   "Access-Control-Allow-Credentials": true,
    //   "Access-Control-Allow-Headers": "Content-Type",
    //   "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
    // }, body: JSON.stringify("Successfully put items")});
  });
}

module.exports.getItems = async event => {
  const roomId = event.queryStringParameters.roomId;
  return await getItems(getItemsParams(roomId));
};

const getItemsParams = (roomId) => {
  return {
    TableName: process.env.ITEMS_TABLE,
    KeyConditionExpression: "roomId = :room",
    ExpressionAttributeValues: {
      ":room": roomId
    }
  };
}

const getItems = params => {
  return new Promise((resolve, reject) => {
    dynamoDb.query(params, function(err, data) {
      if (err) {
        reject({statusCode: 500, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(err)});
      } else {
        resolve({statusCode: 200, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(data)});
      }
    })
  });
}

module.exports.listRooms = async event => {
  return await listItems(listItemsParams());
};

const listItemsParams = () => {
  return {
    TableName: process.env.ITEMS_TABLE
  };
}

const listItems = params => {
  return new Promise((resolve, reject) => {
    dynamoDb.scan(params, function(err, data) {
      if (err) {
        reject({statusCode: 500, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(err)});
      } else {
        resolve({statusCode: 200, headers: {
          "Access-Control-Allow-Origin": "*",
          "Access-Control-Allow-Credentials": true,
          "Access-Control-Allow-Headers": "Content-Type",
          "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
        }, body: JSON.stringify(data)});
      }
    })
  });
}