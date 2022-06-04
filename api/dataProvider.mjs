import fs from 'fs';

const houseDataOutputPath = '../FileSplitter/FileSplitter/bin/Debug/netcoreapp3.1/HouseDataOutput/';

const fileExists = (filename) => {
  try {
    fs.accessSync(houseDataOutputPath + filename + '.csv', fs.constants.R_OK);
    return true;
  } catch (err) {
    console.log(err);
    return false;
  }
}

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

const mapToJson = (houseDataLine) => {
  const items = houseDataLine.split(',');
  
  return {
    price: parseInt(items[0]),
    dateOfTransfer: new Date(items[1]),
    postcode: items[2],
    propertyType: items[3],
    class: items[4],
    tenureDuration: items[5],
    primaryName: items[6],
    secondaryName: items[7],
    street: items[8],
    locality: items[9],
    townOrCity: items[10],
    district: items[11],
    county: items[12]
  };
}

export function getByPostcode(postcode) {
  const postcodeOptions = getPostcodeOptions(postcode.toUpperCase());
  
  if (postcodeOptions.length == 0) {
    return { error: "No matching postcodes found" };
  }

  if (postcodeOptions.length > 1) {
    return { continue: "Please select a postcode prefix", options: postcodeOptions };
  }

  const filePath = `${houseDataOutputPath}${postcodeOptions[0]}.csv`;
  let data = fs
    .readFileSync(filePath, 'utf8')
    .split(/\r\n|\n/);

  data = data
    .filter(x => x.split(',')[2] == postcode)
    .map(mapToJson);

  return { data };
}

export function getByCoordinates(lat, long) {
  return { lat, long };
}