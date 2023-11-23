import dfa.CommonUtils;
import dfa.CustomWebDriverManager;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

public class LoginCreateProfile {

    private WebDriver driver;
    private static String  bceidUSERNAME = System.getenv("USERNAME_BCEID");
    private static String bceidPASSWORD = System.getenv("PASSWORD_BCEID");


    @After
    public void tearDown() {
        driver.close();
        driver.quit();
    }
    @AfterClass
    public static void afterClass() {
        CustomWebDriverManager.instance = null;
    }


    @Test
    public void test() throws Exception {
        driver = CustomWebDriverManager.getDriver();
        WebDriverWait driverWait = CustomWebDriverManager.getDriverWait();
        WebElement element = CustomWebDriverManager.getElement();
        CustomWebDriverManager.getElements();

        CommonUtils.login();
        JavascriptExecutor js = (JavascriptExecutor) driver;
        element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Log in with BC Services Card ')]")));
        js.executeScript("arguments[0].click();", element);
        element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Test with username and password')]")));
        element.click();

        //To be added as sys variables
        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("username")));
        element.sendKeys(bceidUSERNAME);
        //Pass
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
        element.sendKeys(bceidPASSWORD);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("submit-btn")));
        element.click();
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("btnSubmit")));
        element.click();

        //Display notice of Collention
        new WebDriverWait(driver, Duration.ofSeconds(60)).until(
                ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Notice of Collection')]")));



    }
}
