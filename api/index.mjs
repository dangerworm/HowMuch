import express from 'express';
import path from 'path';

import { getByPostcode, getByCoordinates } from './dataProvider.mjs';

const PORT = process.env.PORT || 5000

const parseQueryParams = (req, res) => {
  let postcode = req.query.pc;
  let latitude = req.query.lat;
  let longitude = req.query.long;

  if (postcode) {
    return res.json(getByPostcode(postcode));
  }

  if (latitude && longitude)
  {
    return res.json(getByCoordinates(latitude, longitude));
  }
}

express()
  .use(express.static(path.join(path.dirname('.'), 'public')))
  // .use(bodyParser.urlencoded({ extended: true }))
  // .use(bodyParser.json())
  .get('/', parseQueryParams)
  .listen(PORT, () => console.log(`Listening on ${PORT}`))
