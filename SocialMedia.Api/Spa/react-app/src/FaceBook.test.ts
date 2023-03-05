import { FaceBook } from "./FaceBook";

test('Facebook created successfully', () => {
   new FaceBook()
});

test('Facebook default to undefinded', ()=>{
  var sut = new FaceBook()
  expect(sut.Url).toBe(undefined)
})

test('Facebook set url successfully', ()=>{
  const ulr = "https://www.facebook.com/"
  const sut = new FaceBook()
  sut.Url = new URL(ulr)
  const expected = new URL(ulr)
  expect(sut.Url).toStrictEqual(expected)
})