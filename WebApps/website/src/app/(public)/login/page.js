"use client";
import React, { useState } from "react";
import { motion } from "framer-motion";
import * as yup from "yup";

// Yup schema for phone validation
const phoneSchema = yup.object().shape({
  phone: yup
    .string()
    .required("شماره موبایل الزامی است")
    .matches(/^09\d{9}$/, "شماره موبایل معتبر وارد کنید"),
  role: yup
    .string()
    .oneOf(["seller", "buyer"], "نقش را انتخاب کنید")
    .required("انتخاب نقش الزامی است"),
});
const roles = [
  { value: "seller", label: "فروشنده" },
  { value: "buyer", label: "خریدار" },
];

function PhoneForm({ onSubmit, loading }) {
  const [phone, setPhone] = useState("");
  const [role, setRole] = useState("");
  const [error, setError] = useState("");

  // Check if phone is valid for enabling button
  const isPhoneValid = /^09\d{9}$/.test(phone) && role;

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await phoneSchema.validate({ phone, role });
      onSubmit(phone, role);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <motion.form
      onSubmit={handleSubmit}
      initial={{ opacity: 0, y: 40 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="form"
      style={{
        display: "flex",
        flexDirection: "column",
        gap: 20,
        background: "linear-gradient(135deg, #e3f0ff 0%, #fafcff 100%)",
        padding: 32,
        borderRadius: 18,
        boxShadow: "0 4px 32px #1976d233",
        minWidth: 340,
        maxWidth: 380,
      }}
    >
      <h2 style={{ textAlign: "center", color: "#1976d2", marginBottom: 8 }}>
        ورود به سامانه
      </h2>
      <div style={{ textAlign: "center", color: "#555", fontSize: 15, marginBottom: 8 }}>
        لطفاً نقش خود را انتخاب و شماره موبایل را وارد کنید
      </div>
      <div style={{ display: "flex", gap: 12, justifyContent: "center" }}>
        {roles.map((r) => (
          <button
            key={r.value}
            type="button"
            onClick={() => setRole(r.value)}
            style={{
              flex: 1,
              background: role === r.value ? "#1976d2" : "#f0f4fa",
              color: role === r.value ? "#fff" : "#1976d2",
              border: "none",
              borderRadius: 8,
              padding: "10px 0",
              fontWeight: "bold",
              cursor: "pointer",
              boxShadow: role === r.value ? "0 2px 8px #1976d244" : "none",
              transition: "all 0.2s",
            }}
          >
            {r.label}
          </button>
        ))}
      </div>
      <input
        type="tel"
        placeholder="شماره موبایل"
        value={phone}
        onChange={(e) => setPhone(e.target.value)}
        style={{
          padding: 10,
          borderRadius: 8,
          border: "1px solid #b6c7e3",
          fontSize: 16,
          outline: "none",
          direction: "ltr",
        }}
        dir="ltr"
      />
      {error && (
        <div style={{ color: "red", fontSize: 13, textAlign: "right" }}>{error}</div>
      )}
      <button
        type="submit"
        style={{
          background: "#1976d2",
          color: "#fff",
          border: "none",
          borderRadius: 8,
          padding: 12,
          fontWeight: "bold",
          fontSize: 16,
          cursor: isPhoneValid && !loading ? "pointer" : "not-allowed",
          marginTop: 8,
          boxShadow: "0 2px 8px #1976d244",
          letterSpacing: 1,
          opacity: isPhoneValid && !loading ? 1 : 0.6,
          position: "relative",
        }}
        disabled={!isPhoneValid || loading}
      >
        {loading ? (
          <span
            style={{
              display: "inline-block",
              width: 22,
              height: 22,
              border: "3px solid #fff",
              borderTop: "3px solid #1976d2",
              borderRadius: "50%",
              animation: "spin 1s linear infinite",
              verticalAlign: "middle",
            }}
          />
        ) : (
          "ورود"
        )}
        <style>
          {`
            @keyframes spin {
              0% { transform: rotate(0deg);}
              100% { transform: rotate(360deg);}
            }
          `}
        </style>
      </button>
    </motion.form>
  );
}

function CodeForm({ onSubmit, loading, error }) {
  const [code, setCode] = useState("");
  const isCodeValid = /^\d{4,5}$/.test(code);

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit(code);
  };

  return (
    <motion.form
      onSubmit={handleSubmit}
      initial={{ opacity: 0, y: 40 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      style={{
        display: "flex",
        flexDirection: "column",
        gap: 20,
        background: "linear-gradient(135deg, #e3f0ff 0%, #fafcff 100%)",
        padding: 32,
        borderRadius: 18,
        boxShadow: "0 4px 32px #1976d233",
        minWidth: 340,
        maxWidth: 380,
      }}
    >
      <h2 style={{ textAlign: "center", color: "#1976d2", marginBottom: 8 }}>
        کد پیامک شده را وارد کنید
      </h2>
      <input
        type="tel"
        placeholder="کد پیامک"
        value={code}
        onChange={(e) => setCode(e.target.value)}
        style={{
          padding: 10,
          borderRadius: 8,
          border: "1px solid #b6c7e3",
          fontSize: 16,
          outline: "none",
          direction: "ltr",
          textAlign: "center",
          letterSpacing: 8,
        }}
        dir="ltr"
        maxLength={5}
      />
      {error && (
        <div style={{ color: "red", fontSize: 13, textAlign: "right" }}>{error}</div>
      )}
      <button
        type="submit"
        style={{
          background: "#1976d2",
          color: "#fff",
          border: "none",
          borderRadius: 8,
          padding: 12,
          fontWeight: "bold",
          fontSize: 16,
          cursor: isCodeValid && !loading ? "pointer" : "not-allowed",
          marginTop: 8,
          boxShadow: "0 2px 8px #1976d244",
          letterSpacing: 1,
          opacity: isCodeValid && !loading ? 1 : 0.6,
          position: "relative",
        }}
        disabled={!isCodeValid || loading}
      >
        {loading ? (
          <span
            style={{
              display: "inline-block",
              width: 22,
              height: 22,
              border: "3px solid #fff",
              borderTop: "3px solid #1976d2",
              borderRadius: "50%",
              animation: "spin 1s linear infinite",
              verticalAlign: "middle",
            }}
          />
        ) : (
          "تایید"
        )}
      </button>
      <style>
        {`
          @keyframes spin {
            0% { transform: rotate(0deg);}
            100% { transform: rotate(360deg);}
          }
        `}
      </style>
    </motion.form>
  );
}

// ResendTimer component for handling resend SMS code logic
function ResendTimer({ phone, role, onResend, loading }) {
  const [timer, setTimer] = useState(60);
  const [canResend, setCanResend] = useState(false);

  React.useEffect(() => {
    setTimer(60);
    setCanResend(false);
    const interval = setInterval(() => {
      setTimer((prev) => {
        if (prev <= 1) {
          clearInterval(interval);
          setCanResend(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(interval);
  }, [phone]);

  const handleResend = () => {
    if (canResend && !loading) {
      onResend(phone, role);
    }
  };

  return (
    <div style={{ textAlign: "center", marginBottom: 16, color: "#1976d2", fontSize: 15 }}>
      {canResend ? (
        <button
          onClick={handleResend}
          disabled={loading}
          style={{
            background: "none",
            border: "none",
            color: "#1976d2",
            textDecoration: "underline",
            cursor: loading ? "not-allowed" : "pointer",
            fontSize: 15,
            fontWeight: "bold",
          }}
        >
          ارسال مجدد کد
        </button>
      ) : (
        <span>ارسال مجدد کد تا {timer} ثانیه دیگر</span>
      )}
    </div>
  );
}

function LoginWithPhone() {
  const [step, setStep] = useState("phone"); // phone | code
  const [loading, setLoading] = useState(false);
  const [phone, setPhone] = useState("");
  const [role, setRole] = useState("");
  const [codeError, setCodeError] = useState("");

  // Dummy backend simulation
  const sendCode = (phone, role) => {
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setStep("code");
      setPhone(phone);
      setRole(role);
    }, 1200);
  };

  const verifyCode = (code) => {
    setLoading(true);
    setCodeError("");
    setTimeout(() => {
      setLoading(false);
      // Dummy: accept 0000 or 00000 as valid code
      if (code === "0000" || code === "00000") {
        window.location.href = "/"; // redirect after login
      } else {
        setCodeError("کد وارد شده صحیح نیست");
      }
    }, 1000);
  };

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.4 }}
      style={{
        minHeight: "100vh",
        background: "linear-gradient(120deg, #e3f0ff 0%, #fafcff 100%)",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      {/* Timer and resend logic */}
      {step === "code" && (
        <ResendTimer
          phone={phone}
          role={role}
          onResend={sendCode}
          loading={loading}
        />
      )}
      {step === "phone" ? (
        <PhoneForm
          onSubmit={sendCode}
          loading={loading}
        />
      ) : (
        <CodeForm
          onSubmit={verifyCode}
          loading={loading}
          error={codeError}
        />
      )}
    </motion.div>
  );
}

export default LoginWithPhone;
