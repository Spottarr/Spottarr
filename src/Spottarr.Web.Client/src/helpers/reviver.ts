const dateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z$/;

/* eslint-disable @typescript-eslint/no-explicit-any */
export default function reviver(key: string, value: any) {
  return typeof value === 'string' && dateFormat.test(value) ? new Date(value) : value;
}
