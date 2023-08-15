"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { use, useEffect, useState } from "react";

const page = () => {
  const [redirectparams, setUrl] = useState();
  const searchParams = useSearchParams();
  const router = useRouter();

  useEffect(() => {
    console.log(searchParams.toString());
    fetch(`http://localhost:5039/auth/callback?${searchParams.toString()}`, {
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
    })
      .then((res) => {
        console.log(res);

        debugger
        res.json();
      })
      .then((data) => {
        console.log(data);
        // setUrl(data);
      });
  }, []);

  //   useEffect(() => {
  //     if (router.isReady())
  // }, [])

  return <div>page</div>;
};

export default page;
