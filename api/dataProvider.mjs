export function getByPostcode(postcode) {
  return { result: postcode };
}

export function getByCoordinates(lat, long) {
  return { lat, long };
}