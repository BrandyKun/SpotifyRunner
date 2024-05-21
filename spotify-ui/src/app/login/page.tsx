"use client";
import React, { useEffect, useState } from "react";

type UriResponse = {
  address: string;
};

const Login = () => {
  const [data, setData] = useState<string | null>(null);

  useEffect(() => {
    fetch("https://localhost:7289/api/auth/login")
      .then((res) => res.text()) // Since server returns a plain string
      .then((data) => {
        console.log("Data received from server:", data);
        setData(data);
      })
      .catch((error) => console.error("Error fetching data:", error));
  }, []);

  useEffect(() => {
    if (data) {
      console.log("Redirecting to:", data);
      window.location.replace(data);
    }
  }, [data]);

  return <div>Logging in...</div>;
};

export default Login;
