
'use client'
import React, { useEffect, useState } from 'react'


type UriResponse = {
  address : string
}

const login = () => {
const [data, setData] = useState<UriResponse | null>();

  useEffect(()  => {
    fetch('http://localhost:5039/weatherforecast/authcode', {headers : { 
      'Content-Type': 'application/json',
      'Accept': 'application/json'
     }})
    .then((res) => res.json())
    .then(data => {
      console.log(data);
      setData(data)
    })
  }, [])

  useEffect(() => {
    if(data)
    window.location.replace(data.toString());
  }, [data])
  
  // const res = await fetch('http://localhost:5039/weatherforecast/authcode');
  // const uriRes : UriResponse = await res.json();
  return (
    <div>login</div>

  )
}

export default login