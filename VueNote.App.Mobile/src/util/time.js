import moment from 'moment'
const time = {
    toLocalTime(utcTime) {
        const localTime = moment.utc(utcTime).local().format('YYYY-M-D HH:mm')
        return localTime
    }
}

export default time


