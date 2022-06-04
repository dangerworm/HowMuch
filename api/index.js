const express = require('express');
const path = require('path');

const PORT = process.env.PORT || 5000

const parseQueryParams = (req, res) => {
  let postcode = req.query.pc;
  let latitude = req.query.lat;
  let longitude = req.query.long;

  return res.json({ result: 'Hello, World!' });
}

express()
  .use(express.static(path.join(__dirname, 'public')))
  // .use(bodyParser.urlencoded({ extended: true }))
  // .use(bodyParser.json())
  .get('/', parseQueryParams)
  .listen(PORT, () => console.log(`Listening on ${PORT}`))
