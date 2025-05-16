"use client";
import { useEffect, useRef } from "react";

export default function AnimatedHr() {
  const hrRef = useRef(null);

  useEffect(() => {
    const hr = hrRef.current;
    if (hr) {
      hr.style.width = "0";
      setTimeout(() => {
        hr.style.transition = "width 0.6s";
        hr.style.width = "100%";
      }, 10);
    }
  }, []);

  return (
    <hr
      ref={hrRef}
      className="my-4 border-b-2 border-blue-200"
      style={{ width: 0 }}
    />
  );
}
