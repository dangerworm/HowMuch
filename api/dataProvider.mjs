import fs from 'fs';
import request from 'sync-request';

import { availableFilenames } from './availableFilenames.mjs'
import { adjustPriceByDate } from './priceConverter.mjs';

const localDataPath = '../FileSplitter/FileSplitter/bin/Debug/netcoreapp3.1/HouseDataOutput/';
const remoteDataPath = 'https://raw.githubusercontent.com/dangerworm/HowMuch/main/api/house_price_data/';

const fileExists = (file) => {
  // try {
  //   fs.accessSync(file, fs.constants.R_OK);
  //   return true;
  // } catch (err) {
  //   console.log(err);
  //   return false;
  // }

  return availableFilenames.includes(`${file}.csv`);
}

const getLocalPath = (filename) => `${localDataPath}${filename}.csv`;
const getRemotePath = (filename) => `${remoteDataPath}${filename}.csv`;

const getPostcodeOptions = (postcode) => {
  const filenames = [];
  
  if (postcode.includes(' ') && postcode.split(' ').length > 1) {
    filenames.push(postcode.split(' ')[0]);
  }
  else if (postcode.length > 3) {
    const singleDigitRegex = /[A-Z]{1,2}\d{1}/;
    const doubleDigitRegex = /[A-Z]{1,2}\d{2}/;

    if (singleDigitRegex.test(postcode)) {
      filenames.push(singleDigitRegex.exec(postcode)[0]);
    }

    if (doubleDigitRegex.test(postcode)) {
      filenames.push(doubleDigitRegex.exec(postcode)[0]);
    }
  }

  return filenames.filter(fileExists);
}

const parseLine = (houseDataLine) => {
  const items = houseDataLine.split(',');
  
  return {
    dateOfTransfer: new Date(items[1]),
    price: parseInt(items[0]),
    adjustedPrice: adjustPriceByDate(new Date(items[1]), parseInt(items[0])),
    propertyType: items[3],
    class: items[4],
    tenureDuration: items[5],
    // primaryName: items[6],
    // secondaryName: items[7],
    // street: items[8],
    // locality: items[9],
    // townOrCity: items[10],
    // district: items[11],
    // county: items[12],
    postcode: items[2],
    latitude: parseFloat(items[15]),
    longitude: parseFloat(items[16]),
  };
}

const readLocalFile = (filename) => {
  return fs
    .readFileSync(getLocalPath(filename), 'utf8')
    .split(/\r\n|\n/)
    .map(parseLine);
}

const readRemoteFile = (filename) => {
  return request('GET', getRemotePath(filename))
    .getBody('utf8')
    .split(/\r\n|\n/)
    .map(parseLine);
}

const readFile = (filename, postcode, lat, long) => {
  if (!fileExists(filename)) {
    return [];
  }
  
  //const data = readLocalFile(filename);
  const data = readRemoteFile(filename);

  if (postcode) {
    return data.filter(x => x.postcode == postcode);
  }

  if (lat && long) {
    const latitude = parseFloat(lat);
    const longitude = parseFloat(long);

    return data.filter(x => x.latitude == latitude && x.longitude == longitude);
  }

  return [];
}

const truncate = (value) => {
  return `${Math.trunc(value * 100) / 100}`;
}

export function getByPostcode(postcode) {
  const postcodeOptions = getPostcodeOptions(postcode.toUpperCase());
  
  if (postcodeOptions.length == 0) {
    return { error: "No matching postcode data found" };
  }

  if (postcodeOptions.length > 1) {
    return { continue: "Please select a postcode prefix", options: postcodeOptions };
  }

  if (!postcode.includes(' ')){
    postcode = postcodeOptions[0] + ' ' + postcode.substring(postcodeOptions[0].length);
  }

  const data = readFile(postcodeOptions[0], postcode, null, null);

  return { data };
}

export function getByCoordinates(lat, long) {
  const coordinateRegex = /-?\d+\.\d+/;

  if (!coordinateRegex.test(lat) || !coordinateRegex.test(long)) {
    return { error: "Coordinates in incorrect format" };
  }

  const data = readFile(`${truncate(lat)},${truncate(long)}`, null, lat, long);
  
  return { data };
}